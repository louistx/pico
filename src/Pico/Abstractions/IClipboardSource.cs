using System.Threading.Tasks;

namespace Pico.Abstractions;

public interface IClipboardSource
{
    Task<string?> GetTextAsync();

    Task<string[]> GetFormatsAsync();

    Task<object?> GetDataAsync(string format);
}
