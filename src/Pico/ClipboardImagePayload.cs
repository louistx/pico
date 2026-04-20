using System;
using System.IO;
namespace Pico;

public sealed class ClipboardImagePayload
{
    public ClipboardImagePayload(byte[] imageBytes, string sourceType, string fileExtension)
    {
        ArgumentNullException.ThrowIfNull(imageBytes);
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceType);
        ArgumentException.ThrowIfNullOrWhiteSpace(fileExtension);

        ImageBytes = imageBytes;
        SourceType = sourceType;
        FileExtension = fileExtension;
    }

    public byte[] ImageBytes { get; }

    public string SourceType { get; }

    public string FileExtension { get; }

    public static ClipboardImagePayload FromBitmap(Avalonia.Media.Imaging.Bitmap bitmap, string sourceType, string fileExtension)
    {
        using var stream = new MemoryStream();
        bitmap.Save(stream);
        return new ClipboardImagePayload(stream.ToArray(), sourceType, fileExtension);
    }

    public Avalonia.Media.Imaging.Bitmap CreateBitmap()
    {
        return new Avalonia.Media.Imaging.Bitmap(new MemoryStream(ImageBytes));
    }
}
