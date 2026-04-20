using System.Threading;
using System.Threading.Tasks;
using Pico.Abstractions;

namespace Pico.Infrastructure;

public sealed class NoOpNativeNotificationService : INativeNotificationService
{
    public void Initialize()
    {
    }

    public Task NotifyFileSavedAsync(string filePath, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
