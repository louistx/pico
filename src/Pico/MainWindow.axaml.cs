using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using Pico.Abstractions;
using Pico.Infrastructure;
using Pico.Services;
using Pico.ViewModels;

namespace Pico;

public partial class MainWindow : Window
{
    private readonly ClipboardImportService _clipboardImportService;
    private readonly INativeNotificationService _nativeNotificationService;
    private Avalonia.Media.Imaging.Bitmap? _previewBitmap;

    public MainWindow()
    {
        InitializeComponent();

        var fileNameService = new FileNameService(new SystemClock());
        var fileRevealService = new FileRevealService(new SystemPlatformInfo(), new SystemProcessLauncher());
        _nativeNotificationService = new WindowsNativeNotificationService(fileRevealService);
        _nativeNotificationService.Initialize();
        DataContext = new MainWindowViewModel(
            new JsonAppSettingsStore(),
            new ImageSaveService(fileNameService),
            fileNameService,
            fileRevealService,
            _nativeNotificationService);

        _clipboardImportService = new ClipboardImportService(new ClipboardTextParser());

        AttachKeyBindings();
        ViewModel.Initialize();
    }

    private MainWindowViewModel ViewModel => (MainWindowViewModel)DataContext!;

    private void AttachKeyBindings()
    {
        KeyBindings.Add(new KeyBinding
        {
            Gesture = new KeyGesture(Key.V, KeyModifiers.Control),
            Command = new ActionCommand(LoadFromClipboardAsync)
        });
    }

    private async void PasteButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await LoadFromClipboardAsync();
    }

    private async void SaveButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await ViewModel.SaveAsync();
    }

    private async void OpenSavedFileButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        await ViewModel.RevealLastSavedFileAsync();
    }

    private void DismissNotificationButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        ViewModel.DismissSaveNotification();
    }

    private async void ChooseFolderButton_OnClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var startLocation = await TryGetStartFolderAsync();
        var result = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            AllowMultiple = false,
            SuggestedStartLocation = startLocation,
            Title = "Choose default folder"
        });

        if (result.Count == 0)
        {
            return;
        }

        ViewModel.SetDefaultFolder(result[0].Path.LocalPath);
    }

    private async Task<IStorageFolder?> TryGetStartFolderAsync()
    {
        if (string.IsNullOrWhiteSpace(ViewModel.DefaultFolderPath))
        {
            return null;
        }

        return await StorageProvider.TryGetFolderFromPathAsync(ViewModel.DefaultFolderPath);
    }

    private async Task LoadFromClipboardAsync()
    {
        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        IClipboardSource? source = clipboard is null ? null : new AvaloniaClipboardSource(clipboard);
        var result = await _clipboardImportService.ImportAsync(source);
        UpdatePreview(result);
        ViewModel.ApplyClipboardImport(result);
    }

    private void UpdatePreview(ClipboardImportResult result)
    {
        if (!result.HasPayload)
        {
            return;
        }

        _previewBitmap?.Dispose();
        _previewBitmap = result.Payload!.CreateBitmap();
        PreviewImage.Source = _previewBitmap;
        EmptyPreviewState.IsVisible = false;
    }

    protected override void OnClosed(EventArgs e)
    {
        _previewBitmap?.Dispose();
        if (_nativeNotificationService is IDisposable disposable)
        {
            disposable.Dispose();
        }

        ViewModel.Dispose();
        base.OnClosed(e);
    }
}
