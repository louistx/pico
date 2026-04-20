using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class SpyNativeNotificationService : INativeNotificationService
{
    public int InitializeCallCount { get; private set; }

    public int NotifyCallCount { get; private set; }

    public string? LastPath { get; private set; }

    public void Initialize()
    {
        InitializeCallCount++;
    }

    public Task NotifyFileSavedAsync(string filePath, CancellationToken cancellationToken = default)
    {
        NotifyCallCount++;
        LastPath = filePath;
        return Task.CompletedTask;
    }
}
