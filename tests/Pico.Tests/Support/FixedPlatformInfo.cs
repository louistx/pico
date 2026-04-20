using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class FixedPlatformInfo : IPlatformInfo
{
    public FixedPlatformInfo(AppPlatform current)
    {
        Current = current;
    }

    public AppPlatform Current { get; }
}
