using System.Threading;
using System.Threading.Tasks;

namespace Pico.Abstractions;

public interface INativeNotificationService
{
    void Initialize();

    Task NotifyFileSavedAsync(string filePath, CancellationToken cancellationToken = default);
}
