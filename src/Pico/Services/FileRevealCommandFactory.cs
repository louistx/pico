using System;
using System.IO;
using Pico.Abstractions;

namespace Pico.Services;

public static class FileRevealCommandFactory
{
    public static FileRevealCommand Create(string filePath, AppPlatform platform)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        var normalizedPath = NormalizePath(filePath, platform);
        var directoryPath = GetDirectoryPath(normalizedPath, platform);

        return platform switch
        {
            AppPlatform.Windows => new FileRevealCommand("explorer.exe", $"/select,{Quote(normalizedPath)}"),
            AppPlatform.MacOS => new FileRevealCommand("open", $"-R {Quote(normalizedPath)}"),
            AppPlatform.Linux => new FileRevealCommand("xdg-open", Quote(directoryPath)),
            _ => new FileRevealCommand("xdg-open", Quote(directoryPath))
        };
    }

    private static string NormalizePath(string filePath, AppPlatform platform) =>
        platform == AppPlatform.Windows
            ? Path.GetFullPath(filePath)
            : filePath.Replace('\\', '/');

    private static string GetDirectoryPath(string filePath, AppPlatform platform)
    {
        if (platform == AppPlatform.Windows)
        {
            return Path.GetDirectoryName(filePath) ?? filePath;
        }

        var lastSlashIndex = filePath.LastIndexOf('/');
        return lastSlashIndex > 0
            ? filePath[..lastSlashIndex]
            : filePath;
    }

    private static string Quote(string value) => $"\"{value}\"";
}
