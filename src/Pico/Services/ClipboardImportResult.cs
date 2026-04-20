namespace Pico.Services;

public sealed record ClipboardImportResult(ClipboardImagePayload? Payload, string StatusMessage)
{
    public bool HasPayload => Payload is not null;
}
