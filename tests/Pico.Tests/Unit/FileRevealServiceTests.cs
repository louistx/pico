using Pico.Abstractions;
using Pico.Infrastructure;
using Pico.Tests.Support;

namespace Pico.Tests.Unit;

public sealed class FileRevealServiceTests
{
    [Fact]
    public async Task RevealAsync_StartsProcessWithPlatformCommand()
    {
        var processLauncher = new SpyProcessLauncher();
        var service = new FileRevealService(new FixedPlatformInfo(AppPlatform.Windows), processLauncher);

        await service.RevealAsync(@"C:\temp\img.png");

        Assert.NotNull(processLauncher.LastStartInfo);
        Assert.Equal("explorer.exe", processLauncher.LastStartInfo!.FileName);
        Assert.Equal("/select,\"C:\\temp\\img.png\"", processLauncher.LastStartInfo.Arguments);
    }
}
