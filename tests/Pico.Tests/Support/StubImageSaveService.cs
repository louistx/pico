using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class StubImageSaveService : IImageSaveService
{
    public string SavedPath { get; set; } = string.Empty;

    public Task<string> SaveAsync(
        ClipboardImagePayload payload,
        string folderPath,
        string? fileName,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(SavedPath);
    }
}
