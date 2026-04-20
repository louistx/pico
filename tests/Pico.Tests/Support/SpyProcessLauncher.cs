using System.Diagnostics;
using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class SpyProcessLauncher : IProcessLauncher
{
    public ProcessStartInfo? LastStartInfo { get; private set; }

    public void Start(ProcessStartInfo startInfo)
    {
        LastStartInfo = startInfo;
    }
}
