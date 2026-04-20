using System;
using System.IO;
using Pico.Abstractions;

namespace Pico.Services;

public sealed class FileNameService : IFileNameService
{
    private readonly IClock _clock;

    public FileNameService(IClock clock)
    {
        _clock = clock;
    }

    public string Generate(string extension)
    {
        var safeExtension = NormalizeExtension(extension);
        return $"img-{_clock.Now:yyyy-MM-dd-HHmmss}.{safeExtension}";
    }

    public string Sanitize(string? fileName, string fallbackExtension)
    {
        var safeFallbackExtension = NormalizeExtension(fallbackExtension);
        var candidate = string.IsNullOrWhiteSpace(fileName)
            ? Generate(safeFallbackExtension)
            : fileName.Trim();

        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            candidate = candidate.Replace(invalidChar, '-');
        }

        if (!candidate.Contains('.', StringComparison.Ordinal))
        {
            candidate = $"{candidate}.{safeFallbackExtension}";
        }

        return candidate;
    }

    private static string NormalizeExtension(string extension) =>
        extension.Trim().TrimStart('.').ToLowerInvariant();
}
