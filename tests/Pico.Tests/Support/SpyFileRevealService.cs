using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class SpyFileRevealService : IFileRevealService
{
    public string? LastPath { get; private set; }

    public int CallCount { get; private set; }

    public Task RevealAsync(string filePath, CancellationToken cancellationToken = default)
    {
        LastPath = filePath;
        CallCount++;
        return Task.CompletedTask;
    }
}
