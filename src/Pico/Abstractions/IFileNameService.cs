namespace Pico.Abstractions;

public interface IFileNameService
{
    string Generate(string extension);

    string Sanitize(string? fileName, string fallbackExtension);
}
