using Pico.Services;
using Pico.Tests.Support;

namespace Pico.Tests.Unit;

public sealed class FileNameServiceTests
{
    [Fact]
    public void Generate_UsesClockAndExtension()
    {
        var service = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));

        var fileName = service.Generate("png");

        Assert.Equal("img-2026-04-20-153210.png", fileName);
    }

    [Fact]
    public void Sanitize_ReplacesInvalidChars_AndAddsExtension()
    {
        var service = new FileNameService(new TestClock(new DateTimeOffset(2026, 4, 20, 15, 32, 10, TimeSpan.Zero)));

        var fileName = service.Sanitize("bad:name", "png");

        Assert.Equal("bad-name.png", fileName);
    }
}
