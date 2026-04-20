using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class FakeClipboardSource : IClipboardSource
{
    private readonly Dictionary<string, object> _data;
    private readonly string? _text;

    public FakeClipboardSource(string? text = null, Dictionary<string, object>? data = null)
    {
        _text = text;
        _data = data ?? new Dictionary<string, object>();
    }

    public Task<string?> GetTextAsync() => Task.FromResult(_text);

    public Task<string[]> GetFormatsAsync() => Task.FromResult(_data.Keys.ToArray());

    public Task<object?> GetDataAsync(string format)
    {
        _data.TryGetValue(format, out var value);
        return Task.FromResult(value);
    }
}
