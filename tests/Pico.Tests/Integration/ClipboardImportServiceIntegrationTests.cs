using Pico.Services;
using Pico.Tests.Support;

namespace Pico.Tests.Integration;

public sealed class ClipboardImportServiceIntegrationTests
{
    [Fact]
    [Trait("Category", "Integration")]
    public async Task ImportAsync_WithDataUriClipboard_ReturnsPayload()
    {
        var service = new ClipboardImportService(new ClipboardTextParser());
        var clipboard = new FakeClipboardSource(text: TestImageData.PngDataUri);

        var result = await service.ImportAsync(clipboard);

        Assert.True(result.HasPayload);
        Assert.Equal("png", result.Payload!.FileExtension);
    }

    [Fact]
    [Trait("Category", "Integration")]
    public async Task ImportAsync_WithImageBytes_ReturnsPayload()
    {
        var service = new ClipboardImportService(new ClipboardTextParser());
        var clipboard = new FakeClipboardSource(data: new Dictionary<string, object>
        {
            ["image/png"] = TestImageData.PngBytes
        });

        var result = await service.ImportAsync(clipboard);

        Assert.True(result.HasPayload);
        Assert.Equal("clipboard image", result.Payload!.SourceType);
    }
}
