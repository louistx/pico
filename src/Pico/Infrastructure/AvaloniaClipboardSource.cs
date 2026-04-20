using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Pico.Abstractions;

namespace Pico.Infrastructure;

public sealed class AvaloniaClipboardSource : IClipboardSource
{
    private readonly IClipboard _clipboard;

    public AvaloniaClipboardSource(IClipboard clipboard)
    {
        _clipboard = clipboard;
    }

    public Task<string?> GetTextAsync() => _clipboard.TryGetTextAsync();

    public async Task<string[]> GetFormatsAsync()
    {
        var formats = await _clipboard.GetDataFormatsAsync();
        return formats.Select(static f => f.Identifier).ToArray();
    }

    public async Task<object?> GetDataAsync(string format)
    {
        var data = await _clipboard.TryGetDataAsync();
        if (data is null)
        {
            return null;
        }

        if (string.Equals(format, DataFormat.Text.Identifier, StringComparison.Ordinal))
        {
            return await data.TryGetTextAsync();
        }

        if (string.Equals(format, DataFormat.Bitmap.Identifier, StringComparison.Ordinal))
        {
            return await data.TryGetBitmapAsync();
        }

        var bytes = await data.TryGetValueAsync(DataFormat.CreateBytesPlatformFormat(format));
        if (bytes is not null)
        {
            return bytes;
        }

        return await data.TryGetValueAsync(DataFormat.CreateStringPlatformFormat(format));
    }
}
