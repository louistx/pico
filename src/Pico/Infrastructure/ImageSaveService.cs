using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Pico.Abstractions;

namespace Pico.Infrastructure;

public sealed class ImageSaveService : IImageSaveService
{
    private readonly IFileNameService _fileNameService;

    public ImageSaveService(IFileNameService fileNameService)
    {
        _fileNameService = fileNameService;
    }

    public async Task<string> SaveAsync(
        ClipboardImagePayload payload,
        string folderPath,
        string? fileName,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(folderPath);

        var safeFileName = _fileNameService.Sanitize(fileName, payload.FileExtension);
        var fullPath = Path.Combine(folderPath, safeFileName);

        await File.WriteAllBytesAsync(fullPath, payload.ImageBytes, cancellationToken);

        return fullPath;
    }
}
