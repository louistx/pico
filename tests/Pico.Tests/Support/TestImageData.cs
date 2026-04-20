using System;

namespace Pico.Tests.Support;

internal static class TestImageData
{
    public const string PngBase64 =
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAANSURBVBhXY/jPwPAfAAUAAf+mXJtdAAAAAElFTkSuQmCC";

    public static byte[] PngBytes => Convert.FromBase64String(PngBase64);

    public static string PngDataUri => $"data:image/png;base64,{PngBase64}";
}
