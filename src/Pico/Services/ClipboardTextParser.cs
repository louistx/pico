using System;
using System.Text.RegularExpressions;

namespace Pico.Services;

public sealed class ClipboardTextParser
{
    private static readonly Regex DataUriRegex = new(
        @"^\s*data:image/(?<format>[a-zA-Z0-9.+-]+);base64,(?<data>[a-zA-Z0-9+/=\r\n]+)\s*$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public ClipboardImagePayload? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        if (TryParseDataUri(text, out var dataUriPayload))
        {
            return dataUriPayload;
        }

        if (TryParseRawBase64(text, out var base64Payload))
        {
            return base64Payload;
        }

        return null;
    }

    private static bool TryParseDataUri(string text, out ClipboardImagePayload? payload)
    {
        var match = DataUriRegex.Match(text.Trim());
        if (!match.Success)
        {
            payload = null;
            return false;
        }

        var format = NormalizeFormat(match.Groups["format"].Value);
        var base64 = CompactBase64(match.Groups["data"].Value);
        return TryBuildPayload(base64, $"data URI ({format})", format, out payload);
    }

    private static bool TryParseRawBase64(string text, out ClipboardImagePayload? payload)
    {
        var candidate = CompactBase64(text);
        if (candidate.Length < 64 || candidate.Length % 4 != 0)
        {
            payload = null;
            return false;
        }

        return TryBuildPayload(candidate, "raw base64", "png", out payload);
    }

    private static bool TryBuildPayload(
        string base64,
        string sourceType,
        string preferredFormat,
        out ClipboardImagePayload? payload)
    {
        try
        {
            var bytes = Convert.FromBase64String(base64);
            if (!ImageFormatDetector.TryDetectExtension(bytes, out var detectedFormat))
            {
                payload = null;
                return false;
            }

            var fileExtension = string.IsNullOrWhiteSpace(preferredFormat) ? detectedFormat : preferredFormat;
            payload = new ClipboardImagePayload(bytes, sourceType, fileExtension);
            return true;
        }
        catch
        {
            payload = null;
            return false;
        }
    }

    private static string CompactBase64(string value) =>
        value.Replace("\r", string.Empty)
            .Replace("\n", string.Empty)
            .Trim();

    private static string NormalizeFormat(string format)
    {
        var normalized = format.Trim().ToLowerInvariant();
        return normalized switch
        {
            "jpeg" => "jpg",
            "svg+xml" => "png",
            _ => normalized
        };
    }
}
