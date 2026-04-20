using Pico.Infrastructure;
using Pico.Tests.Support;

namespace Pico.Tests.Unit;

public sealed class WindowsProtocolActivationTests
{
    [Fact]
    public void BuildOpenSavedFileUri_EncodesPath()
    {
        var uri = WindowsProtocolActivation.BuildOpenSavedFileUri(@"C:\temp\img 01.png");

        Assert.Equal("pico://opensavedfile/?path=C%3A%5Ctemp%5Cimg%2001.png", uri.AbsoluteUri);
    }

    [Fact]
    public void TryHandle_ValidProtocol_RevealsFile()
    {
        var revealService = new SpyFileRevealService();

        var handled = WindowsProtocolActivation.TryHandle(
            new[] { "pico://openSavedFile?path=C%3A%5Ctemp%5Cimg.png" },
            revealService);

        Assert.True(handled);
        Assert.Equal(@"C:\temp\img.png", revealService.LastPath);
    }

    [Fact]
    public void TryHandle_NonProtocol_Ignores()
    {
        var revealService = new SpyFileRevealService();

        var handled = WindowsProtocolActivation.TryHandle(new[] { "--normal" }, revealService);

        Assert.False(handled);
        Assert.Null(revealService.LastPath);
    }
}
