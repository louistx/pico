using Pico.Services;
using Pico.Tests.Support;

namespace Pico.Tests.Unit;

public sealed class ClipboardTextParserTests
{
    private readonly ClipboardTextParser _parser = new();

    [Fact]
    public void Parse_DataUri_ReturnsPayload()
    {
        var payload = _parser.Parse(TestImageData.PngDataUri);

        Assert.NotNull(payload);
        Assert.Equal("png", payload.FileExtension);
        Assert.Equal("data URI (png)", payload.SourceType);
    }

    [Fact]
    public void Parse_RawBase64_ReturnsPayload()
    {
        var payload = _parser.Parse(TestImageData.PngBase64);

        Assert.NotNull(payload);
        Assert.Equal("png", payload.FileExtension);
        Assert.Equal("raw base64", payload.SourceType);
    }

    [Fact]
    public void Parse_InvalidText_ReturnsNull()
    {
        var payload = _parser.Parse("not image");

        Assert.Null(payload);
    }
}
