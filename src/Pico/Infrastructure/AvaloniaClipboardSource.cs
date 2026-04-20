using System.Threading.Tasks;
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

    public Task<string?> GetTextAsync() => _clipboard.GetTextAsync();

    public Task<string[]> GetFormatsAsync() => _clipboard.GetFormatsAsync();

    public Task<object?> GetDataAsync(string format) => _clipboard.GetDataAsync(format);
}
