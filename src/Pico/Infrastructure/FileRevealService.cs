using System.Diagnostics;
using Pico.Abstractions;
using Pico.Services;

namespace Pico.Infrastructure;

public sealed class FileRevealService : IFileRevealService
{
    private readonly IPlatformInfo _platformInfo;
    private readonly IProcessLauncher _processLauncher;

    public FileRevealService(IPlatformInfo platformInfo, IProcessLauncher processLauncher)
    {
        _platformInfo = platformInfo;
        _processLauncher = processLauncher;
    }

    public Task RevealAsync(string filePath, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var command = FileRevealCommandFactory.Create(filePath, _platformInfo.Current);
        var startInfo = new ProcessStartInfo
        {
            FileName = command.FileName,
            Arguments = command.Arguments,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _processLauncher.Start(startInfo);
        return Task.CompletedTask;
    }
}
