using System.Diagnostics;
using Pico.Abstractions;

namespace Pico.Infrastructure;

public sealed class SystemProcessLauncher : IProcessLauncher
{
    public void Start(ProcessStartInfo startInfo)
    {
        Process.Start(startInfo);
    }
}
