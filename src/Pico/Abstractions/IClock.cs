using System;

namespace Pico.Abstractions;

public interface IClock
{
    DateTimeOffset Now { get; }
}
