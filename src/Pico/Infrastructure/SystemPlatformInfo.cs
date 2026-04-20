using System.Runtime.InteropServices;
using Pico.Abstractions;

namespace Pico.Infrastructure;

public sealed class SystemPlatformInfo : IPlatformInfo
{
    public AppPlatform Current =>
        RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? AppPlatform.Windows :
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? AppPlatform.MacOS :
        RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? AppPlatform.Linux :
        AppPlatform.Unknown;
}
