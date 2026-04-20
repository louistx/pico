# Contributing to PICO

Thanks for helping improve PICO.

## Product guardrails

PICO should stay:

- lightweight
- fast
- focused on one job
- easy to understand
- pleasant to use

Before adding anything, ask whether it makes the core flow faster or clearer:

1. Paste image-like clipboard content.
2. Preview it immediately.
3. Save it in seconds.

If a change pushes the app toward being an editor, clipboard manager, or general-purpose image tool, it is probably outside scope.

## Development principles

- Keep responsibilities small and explicit.
- Prefer testable services over UI-driven logic.
- Keep UI code thin.
- Preserve cross-platform behavior where possible.
- Add unit or integration tests for meaningful behavior changes.

## Local workflow

```powershell
dotnet build Pico.sln
dotnet test Pico.sln
```

Run on Windows:

```powershell
dotnet run --framework net8.0-windows10.0.17763.0 --project C:\path\to\pico\src\Pico\Pico.csproj
```

Run on Linux or macOS:

```bash
dotnet run --framework net8.0 --project ./src/Pico/Pico.csproj
```

## Pull requests

Please keep pull requests focused and easy to review.

Good pull requests usually include:

- a clear problem statement
- a concise change summary
- tests or rationale when tests are not practical
- screenshots or short recordings for visible UI changes

## Discussions

If you are unsure whether a feature fits the product direction, open an issue first and describe the user pain it solves.
