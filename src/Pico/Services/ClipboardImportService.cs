using System;
using System.Threading.Tasks;
using Pico.Abstractions;

namespace Pico.Services;

public sealed class ClipboardImportService
{
    private readonly ClipboardTextParser _textParser;

    public ClipboardImportService(ClipboardTextParser textParser)
    {
        _textParser = textParser;
    }

    public async Task<ClipboardImportResult> ImportAsync(IClipboardSource? clipboard)
    {
        try
        {
            if (clipboard is null)
            {
                return new ClipboardImportResult(null, "Clipboard unavailable.");
            }

            var imagePayload = await TryReadImageAsync(clipboard);
            if (imagePayload is not null)
            {
                return new ClipboardImportResult(imagePayload, $"{imagePayload.SourceType} detected. Ready to save.");
            }

            var text = await clipboard.GetTextAsync();
            var textPayload = _textParser.Parse(text);
            if (textPayload is not null)
            {
                return new ClipboardImportResult(textPayload, $"{textPayload.SourceType} detected. Ready to save.");
            }

            return new ClipboardImportResult(null, "Clipboard has no valid image or usable base64.");
        }
        catch (Exception ex)
        {
            return new ClipboardImportResult(null, $"Clipboard read failed: {ex.Message}");
        }
    }

    private static async Task<ClipboardImagePayload?> TryReadImageAsync(IClipboardSource clipboard)
    {
        var formats = await clipboard.GetFormatsAsync();
        foreach (var format in formats)
        {
            var data = await clipboard.GetDataAsync(format);

            if (data is Avalonia.Media.Imaging.Bitmap bitmap)
            {
                return ClipboardImagePayload.FromBitmap(bitmap, "clipboard image", "png");
            }

            if (data is byte[] rawBytes && ImageFormatDetector.TryDetectExtension(rawBytes, out var extension))
            {
                return new ClipboardImagePayload(rawBytes, "clipboard image", extension);
            }
        }

        return null;
    }
}
