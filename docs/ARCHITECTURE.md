# Architecture Notes

## Goals

The implementation should stay:

- readable
- easy to extend
- testable
- intentionally small

## Current structure

### `src/Pico/Abstractions`

Small interfaces that isolate platform-dependent behavior and infrastructure concerns.

Examples:

- settings persistence
- clipboard input
- file reveal behavior
- native notification behavior
- clock/process/platform abstractions

### `src/Pico/Services`

Application rules and focused logic.

Examples:

- clipboard text parsing
- import flow
- file naming
- image format detection
- file reveal command construction

### `src/Pico/Infrastructure`

OS and framework integrations.

Examples:

- Avalonia clipboard adapter
- JSON settings storage
- file save service
- process launching
- Windows native notification implementation

### `src/Pico/ViewModels`

UI-facing state and orchestration logic.

The `MainWindowViewModel` owns user-visible state like:

- detected type
- suggested format
- save status
- default folder
- save notification state

### UI layer

`MainWindow.axaml` and `MainWindow.axaml.cs` are intentionally thin:

- wiring
- file picker
- preview rendering
- keyboard handling

## Testing strategy

### Unit tests

Used for pure logic:

- parsing
- naming
- command generation
- view-model state transitions

### Integration tests

Used for realistic flows:

- import from fake clipboard sources
- save to temporary folders
- verify end-to-end behavior without relying on a real desktop session

## Design decisions

- Keep clipboard payloads as image bytes in the domain layer
- Convert to `Bitmap` only at the UI edge for preview
- Isolate platform-specific work behind interfaces
- Prefer explicit services over giant code-behind classes
- Keep the product focused on one core workflow
