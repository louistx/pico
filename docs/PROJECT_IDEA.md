# Project Idea

## One-line pitch

Paste any image-like clipboard content and save it as a file in seconds.

## Problem

This is a small pain, but it happens constantly.

Typical flow today:

1. Copy an image or screenshot.
2. Open a heavy editor or random app.
3. Paste.
4. Save.
5. Choose a name, folder, and format.

For base64 image payloads, the flow is usually much worse. Developers, designers, QA, support people, and power users often receive raw image data that should become a file quickly, but the available tools are either bloated, awkward, or not built for that job.

## Product thesis

There is real value in a utility that is:

- lightweight
- instant
- focused on one job
- visually polished
- almost zero-friction

The goal is not to build another editor. The goal is to build a habit-forming utility.

## Core positioning

> Paste any image from your clipboard and save it in two seconds.

## MVP scope

- Small main window
- Dedicated paste area
- Immediate preview
- Save button
- Clipboard detection for:
  - real bitmap/image content
  - raw base64 image strings
  - `data:image/...;base64,...`
- Default save folder
- Automatic file naming such as `img-2026-04-20-153210.png`

## UX principles

- Fast before fancy
- No forced workflow
- No surprise auto-save behavior
- No editing surface unless it clearly supports the main job
- Great defaults reduce decision fatigue

## Nice-to-have features after MVP

- More output formats such as JPG and WebP
- A short history of recent clipboard items
- Tray mode
- Better first-run setup
- Better drag-and-drop support

## Non-goals

- Becoming an image editor
- Becoming a clipboard manager
- Becoming a DAM or file browser
- Accumulating features that slow down the main flow

## Risks and tricky details

- Windows clipboard formats can be inconsistent
- Base64 payloads can be malformed or incomplete
- Transparency should usually default to PNG, not JPG
- "Automatic" clipboard watchers can become intrusive quickly
- Folder and file naming defaults must feel right

## Technical direction

- C#
- Avalonia
- Small service-oriented architecture
- Unit and integration test coverage around core flows

## Name

**PICO**: Paste Image, Create Output
