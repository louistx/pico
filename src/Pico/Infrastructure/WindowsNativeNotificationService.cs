using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pico.Abstractions;

namespace Pico.Infrastructure;

#if WINDOWS
using System.Runtime.InteropServices;
using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

public sealed class WindowsNativeNotificationService : INativeNotificationService
{
    private const string AppUserModelId = "Pico.PasteImageCreateOutput";
    private static readonly Guid ToastActivatorClsid = new("8B0671DB-0787-46A1-BE57-6D9A0D4B9D77");
    private static readonly string LogPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Pico", "native-notification.log");

    private bool _initialized;

    public WindowsNativeNotificationService(IFileRevealService fileRevealService)
    {
    }

    public void Initialize()
    {
        if (_initialized)
        {
            return;
        }

        try
        {
            var executablePath = ResolveExecutablePath();
            Log($"Initialize start. exe={executablePath}");
            WindowsProtocolActivation.Register(executablePath);
            Log("Protocol registered.");
            WindowsStartMenuShortcut.EnsureCreated(executablePath, AppUserModelId, ToastActivatorClsid);
            Log("Start menu shortcut ensured.");
            _initialized = true;
            Log("Initialize success.");
        }
        catch (Exception ex)
        {
            _initialized = false;
            Log($"Initialize failed: {ex}");
        }
    }

    public Task NotifyFileSavedAsync(string filePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_initialized)
        {
            return Task.CompletedTask;
        }

        try
        {
            var activationUri = WindowsProtocolActivation.BuildOpenSavedFileUri(filePath);
            Log($"Notify start. file={filePath} uri={activationUri}");
            var content = new ToastContentBuilder()
                .AddText("Arquivo salvo")
                .AddText(Path.GetFileName(filePath))
                .SetProtocolActivation(activationUri)
                .AddButton(new ToastButton()
                    .SetContent("Abrir na pasta")
                    .SetProtocolActivation(activationUri));

            var notification = new ToastNotification(content.Content.GetXml());
            ToastNotificationManager.CreateToastNotifier(AppUserModelId).Show(notification);
            Log("Toast show requested.");
        }
        catch (Exception ex)
        {
            Log($"Notify failed: {ex}");
        }

        return Task.CompletedTask;
    }

    private static string ResolveExecutablePath() =>
        Environment.ProcessPath
        ?? throw new InvalidOperationException("Process path unavailable.");

    private static void Log(string message)
    {
        try
        {
            var directory = Path.GetDirectoryName(LogPath)!;
            Directory.CreateDirectory(directory);
            File.AppendAllText(LogPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}");
        }
        catch
        {
        }
    }
}

public static class WindowsProtocolActivation
{
    private const string ProtocolName = "pico";

    public static void Register(string executablePath)
    {
        using var protocolKey = Microsoft.Win32.Registry.CurrentUser.CreateSubKey($@"Software\Classes\{ProtocolName}");
        protocolKey?.SetValue(string.Empty, "URL:PICO Protocol");
        protocolKey?.SetValue("URL Protocol", string.Empty);

        using var defaultIconKey = protocolKey?.CreateSubKey("DefaultIcon");
        defaultIconKey?.SetValue(string.Empty, $"\"{executablePath}\",0");

        using var commandKey = protocolKey?.CreateSubKey(@"shell\open\command");
        commandKey?.SetValue(string.Empty, $"\"{executablePath}\" \"%1\"");
    }

    public static Uri BuildOpenSavedFileUri(string filePath)
    {
        var encodedPath = Uri.EscapeDataString(filePath);
        return new Uri($"{ProtocolName}://openSavedFile?path={encodedPath}");
    }

    public static bool TryHandle(string[] args, IFileRevealService fileRevealService)
    {
        if (args.Length == 0 || !Uri.TryCreate(args[0], UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!string.Equals(uri.Scheme, ProtocolName, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.Host, "openSavedFile", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var path = TryReadQueryValue(uri.Query, "path");
        if (string.IsNullOrWhiteSpace(path))
        {
            return true;
        }

        fileRevealService.RevealAsync(path).GetAwaiter().GetResult();
        return true;
    }

    private static string? TryReadQueryValue(string query, string key)
    {
        var trimmedQuery = query.TrimStart('?');
        foreach (var part in trimmedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var pieces = part.Split('=', 2);
            if (pieces.Length != 2)
            {
                continue;
            }

            if (!string.Equals(pieces[0], key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return Uri.UnescapeDataString(pieces[1]);
        }

        return null;
    }
}

internal static class WindowsStartMenuShortcut
{
    private static readonly PropertyKey AppUserModelIdKey = new(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 5);
    private static readonly PropertyKey ToastActivatorClsidKey = new(new Guid("9F4C2855-9F79-4B39-A8D0-E1D42DE1D5F3"), 26);

    public static void EnsureCreated(string executablePath, string appUserModelId, Guid toastActivatorClsid)
    {
        var programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
        var shortcutPath = Path.Combine(programsFolder, "PICO.lnk");

        var shellLinkType = Type.GetTypeFromCLSID(new Guid("00021401-0000-0000-C000-000000000046"))
            ?? throw new InvalidOperationException("ShellLink COM type unavailable.");
        var shellLink = (IShellLinkW)Activator.CreateInstance(shellLinkType)!;
        try
        {
            shellLink.SetPath(executablePath);
            shellLink.SetArguments(string.Empty);
            shellLink.SetWorkingDirectory(Path.GetDirectoryName(executablePath)!);
            shellLink.SetIconLocation(executablePath, 0);

            var propertyStore = (IPropertyStore)shellLink;
            var appUserModelIdKey = AppUserModelIdKey;
            using var appIdValue = PropVariant.FromString(appUserModelId);
            propertyStore.SetValue(ref appUserModelIdKey, appIdValue);

            var toastActivatorClsidKey = ToastActivatorClsidKey;
            using var clsidValue = PropVariant.FromGuid(toastActivatorClsid);
            propertyStore.SetValue(ref toastActivatorClsidKey, clsidValue);
            propertyStore.Commit();

            var persistFile = (IPersistFile)shellLink;
            persistFile.Save(shortcutPath, true);
        }
        finally
        {
            Marshal.FinalReleaseComObject(shellLink);
        }
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    private interface IShellLinkW
    {
        void GetPath([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszFile, int cchMaxPath, IntPtr pfd, uint fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszName, int cchMaxName);
        void SetDescription([MarshalAs(UnmanagedType.LPWStr)] string pszName);
        void GetWorkingDirectory([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory([MarshalAs(UnmanagedType.LPWStr)] string pszDir);
        void GetArguments([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszArgs, int cchMaxPath);
        void SetArguments([MarshalAs(UnmanagedType.LPWStr)] string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation([Out, MarshalAs(UnmanagedType.LPWStr)] System.Text.StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation([MarshalAs(UnmanagedType.LPWStr)] string pszIconPath, int iIcon);
        void SetRelativePath([MarshalAs(UnmanagedType.LPWStr)] string pszPathRel, uint dwReserved);
        void Resolve(IntPtr hwnd, uint fFlags);
        void SetPath([MarshalAs(UnmanagedType.LPWStr)] string pszFile);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("886D8EEB-8CF2-4446-8D02-CDBA1DBDCF99")]
    private interface IPropertyStore
    {
        void GetCount(out uint cProps);
        void GetAt(uint iProp, out PropertyKey pkey);
        void GetValue(ref PropertyKey key, out PropVariant pv);
        void SetValue(ref PropertyKey key, PropVariant pv);
        void Commit();
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0000010B-0000-0000-C000-000000000046")]
    private interface IPersistFile
    {
        void GetClassID(out Guid pClassID);
        void IsDirty();
        void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
        void Save([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, bool fRemember);
        void SaveCompleted([MarshalAs(UnmanagedType.LPWStr)] string pszFileName);
        void GetCurFile([MarshalAs(UnmanagedType.LPWStr)] out string ppszFileName);
    }

    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    private readonly struct PropertyKey
    {
        public PropertyKey(Guid formatId, uint propertyId)
        {
            FormatId = formatId;
            PropertyId = propertyId;
        }

        public Guid FormatId { get; }

        public uint PropertyId { get; }
    }

    [StructLayout(LayoutKind.Explicit)]
    private sealed class PropVariant : IDisposable
    {
        [FieldOffset(0)]
        private ushort _valueType;

        [FieldOffset(8)]
        private IntPtr _pointerValue;

        private PropVariant()
        {
        }

        public static PropVariant FromString(string value)
        {
            return new PropVariant
            {
                _valueType = 31,
                _pointerValue = Marshal.StringToCoTaskMemUni(value)
            };
        }

        public static PropVariant FromGuid(Guid value)
        {
            var pointer = Marshal.AllocCoTaskMem(Marshal.SizeOf<Guid>());
            Marshal.StructureToPtr(value, pointer, false);
            return new PropVariant
            {
                _valueType = 72,
                _pointerValue = pointer
            };
        }

        public void Dispose()
        {
            PropVariantClear(this);
            GC.SuppressFinalize(this);
        }

        ~PropVariant()
        {
            PropVariantClear(this);
        }

        [DllImport("ole32.dll")]
        private static extern int PropVariantClear([In, Out] PropVariant propvar);
    }
}
#else
public sealed class WindowsNativeNotificationService : INativeNotificationService
{
    public WindowsNativeNotificationService(IFileRevealService fileRevealService)
    {
    }

    public void Initialize()
    {
    }

    public Task NotifyFileSavedAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}

public static class WindowsProtocolActivation
{
    private const string ProtocolName = "pico";

    public static Uri BuildOpenSavedFileUri(string filePath)
    {
        var encodedPath = Uri.EscapeDataString(filePath);
        return new Uri($"{ProtocolName}://openSavedFile?path={encodedPath}");
    }

    public static bool TryHandle(string[] args, IFileRevealService fileRevealService)
    {
        if (args.Length == 0 || !Uri.TryCreate(args[0], UriKind.Absolute, out var uri))
        {
            return false;
        }

        if (!string.Equals(uri.Scheme, ProtocolName, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(uri.Host, "openSavedFile", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        var path = TryReadQueryValue(uri.Query, "path");
        if (string.IsNullOrWhiteSpace(path))
        {
            return true;
        }

        fileRevealService.RevealAsync(path).GetAwaiter().GetResult();
        return true;
    }

    private static string? TryReadQueryValue(string query, string key)
    {
        var trimmedQuery = query.TrimStart('?');
        foreach (var part in trimmedQuery.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var pieces = part.Split('=', 2);
            if (pieces.Length != 2)
            {
                continue;
            }

            if (!string.Equals(pieces[0], key, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            return Uri.UnescapeDataString(pieces[1]);
        }

        return null;
    }
}
#endif
