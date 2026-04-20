using System.Diagnostics;

namespace Pico.Abstractions;

public interface IProcessLauncher
{
    void Start(ProcessStartInfo startInfo);
}
