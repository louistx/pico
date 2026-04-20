using System.Threading;
using System.Threading.Tasks;

namespace Pico.Abstractions;

public interface IImageSaveService
{
    Task<string> SaveAsync(
        ClipboardImagePayload payload,
        string folderPath,
        string? fileName,
        CancellationToken cancellationToken = default);
}
