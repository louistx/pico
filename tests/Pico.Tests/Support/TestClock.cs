using System;
using Pico.Abstractions;

namespace Pico.Tests.Support;

internal sealed class TestClock : IClock
{
    public TestClock(DateTimeOffset now)
    {
        Now = now;
    }

    public DateTimeOffset Now { get; }
}
