using Pico.Abstractions;
using Pico.Services;

namespace Pico.Tests.Unit;

public sealed class FileRevealCommandFactoryTests
{
    [Fact]
    public void Create_Windows_UsesExplorerSelect()
    {
        var command = FileRevealCommandFactory.Create(@"C:\temp\img.png", AppPlatform.Windows);

        Assert.Equal("explorer.exe", command.FileName);
        Assert.Equal("/select,\"C:\\temp\\img.png\"", command.Arguments);
    }

    [Fact]
    public void Create_Mac_UsesOpenReveal()
    {
        var command = FileRevealCommandFactory.Create("/tmp/img.png", AppPlatform.MacOS);

        Assert.Equal("open", command.FileName);
        Assert.Equal("-R \"/tmp/img.png\"", command.Arguments);
    }

    [Fact]
    public void Create_Linux_OpensDirectory()
    {
        var command = FileRevealCommandFactory.Create("/tmp/images/img.png", AppPlatform.Linux);

        Assert.Equal("xdg-open", command.FileName);
        Assert.Equal("\"/tmp/images\"", command.Arguments);
    }
}
