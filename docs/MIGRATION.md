# Migration Guide

This guide covers migration from legacy `System.Drawing`-first plugin flows to the new compatibility model in `StreamDeck-Tools`.

## Goals

- Existing plugins should continue to work with minimal or no code changes in common scenarios.
- New development should move to non-`System.Drawing`-centric APIs over time.
- Migration should be incremental and predictable.

## Upgrade Paths

### Path A: No-Code-Change First

- Upgrade package version.
- Keep existing calls like `SetImageAsync(Image image, ...)` while validating runtime behavior.
- If your plugin only uses library helpers for image/title handling, this is the recommended first step.

### Path B: Proactive Modernization

- Move to base64/byte-based APIs for image updates.
- Reduce direct usage of `System.Drawing` in plugin code.
- Adopt new non-`System.Drawing` APIs as they are added.

## API Mapping Template

Use this template while migrating plugin code:

| Current API | New API | Migration effort | Notes |
| --- | --- | --- | --- |
| `SetImageAsync(Image, ...)` | `SetImageAsync(string base64Image, ...)` | rename + conversion | Convert image once and reuse encoded payload |
| `Tools.ImageToBase64(Image, ...)` | `Tools.ImageToBase64(Image, ...)` | no change | Legacy-compatible path retained |
| `Tools.Base64StringToImage(string)` | `Tools.Base64StringToImage(string)` | no change | Legacy-compatible path retained |
| `GraphicsTools.*` (`Image`/`Bitmap`) | upcoming backend-neutral equivalents | small-to-medium | Prefer new methods when available |

## Direct System.Drawing Detection Checklist

Scan plugin code for direct dependencies:

- `using System.Drawing`
- `new Bitmap(...)`
- `Graphics.FromImage(...)`
- `Image.FromFile(...)` / `Image.FromStream(...)`
- `Font`, `FontFamily`, `FontStyle`, `Brush`, `SolidBrush`

Suggested search commands:

```bash
rg "using System\\.Drawing" --type cs
rg "Bitmap\\(|Graphics\\.FromImage|Image\\.From(File|Stream)" --type cs
rg "\\bFont\\b|FontFamily|FontStyle|SolidBrush|Brush\\b" --type cs
```

## Deprecation Timeline

- **Release N**: legacy APIs remain supported without forced warnings.
- **Release N+1**: targeted `[Obsolete]` warnings start, each with clear replacement guidance.
- **Next major**: removal candidates are evaluated after adoption feedback and compatibility results.

## Compatibility Notes

- Exact pixel parity is not guaranteed for every text/layout edge case across rendering backends.
- Functional parity is the target for common plugin scenarios (resize, encoding, title/image updates).
- Plugins with heavy direct `System.Drawing` usage may require focused migration work.
