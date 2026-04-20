using System.Threading;
using System.Threading.Tasks;

namespace Pico.Abstractions;

public interface IFileRevealService
{
    Task RevealAsync(string filePath, CancellationToken cancellationToken = default);
}
