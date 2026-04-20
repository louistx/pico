# PICO

[![CI](https://github.com/louistx/pico/actions/workflows/ci.yml/badge.svg)](https://github.com/louistx/pico/actions/workflows/ci.yml)
[![Release](https://github.com/louistx/pico/actions/workflows/release.yml/badge.svg)](https://github.com/louistx/pico/actions/workflows/release.yml)

PICO is a lightweight desktop utility for one annoying but frequent job: take whatever image-like thing is in your clipboard and turn it into a real file in seconds.

It is built for people who constantly bounce between screenshots, copied images, `data:image/...;base64,...` strings, and random image payloads that should have been files already.

## Why PICO exists

This workflow is way more annoying than it should be:

1. Copy an image.
2. Open a heavy editor or viewer.
3. Paste it.
4. Save it.
5. Pick a folder, file name, and format.

Base64 is even worse. A lot of us receive image data URIs or raw base64 blobs and simply need to materialize them into a file without ceremony.

PICO focuses on removing friction instead of adding features.

## What it does

- Paste directly from the clipboard with `Ctrl+V`
- Detect real clipboard images
- Detect raw base64 image payloads
- Detect `data:image/...;base64,...` strings
- Show an instant preview
- Save with one click
- Keep a default output folder
- Generate timestamp-based file names automatically
- Show in-app save feedback
- On Windows, raise a native notification and open the saved file in Explorer when clicked

## What it does not try to be

PICO is intentionally not:

- an image editor
- a clipboard history manager
- a mini Photoshop
- a file organizer

The product direction is simple: **paste any image-like clipboard content and save it in two seconds**.

## Current status

PICO is in active MVP development. The core save flow is already working and covered by automated tests.

Implemented today:

- Avalonia desktop UI
- Clipboard import pipeline
- Base64 and data URI parsing
- Default folder persistence
- Automatic file naming
- File reveal integration
- Windows native notification integration
- Unit and integration tests
- GitHub Actions for CI and releases

## Platform support

- Windows: first-class support, including native save notification behavior
- Linux: core app flow supported
- macOS: core app flow supported

Release artifacts are published through GitHub Releases.

## Download

Prebuilt binaries are available on the [Releases](https://github.com/louistx/pico/releases) page.

## Development

### Requirements

- .NET 8 SDK

### Run locally

```powershell
dotnet run --framework net8.0-windows10.0.17763.0 --project C:\path\to\pico\src\Pico\Pico.csproj
```

On non-Windows platforms:

```bash
dotnet run --framework net8.0 --project ./src/Pico/Pico.csproj
```

### Build

```powershell
dotnet build Pico.sln
```

### Test

```powershell
dotnet test Pico.sln
```

## Project docs

- [Project Idea](docs/PROJECT_IDEA.md)
- [Architecture Notes](docs/ARCHITECTURE.md)
- [Roadmap](docs/ROADMAP.md)
- [Changelog](CHANGELOG.md)

## Roadmap highlights

- More export formats such as JPG and WebP
- Better format inference and transparency-aware defaults
- Optional tray mode
- Small clipboard history
- Polished onboarding and first-run setup

## Contributing

The codebase is being kept intentionally small, readable, and testable. If you want to contribute, start by reading the docs above and keep changes aligned with the core product philosophy: fast, focused, and low-friction.

## License

No license has been added yet. That decision should be intentional before public reuse is allowed.
