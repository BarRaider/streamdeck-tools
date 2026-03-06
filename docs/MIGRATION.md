# Migration Guide

This guide covers migration from legacy `System.Drawing`-first plugin flows to the new compatibility model in `StreamDeck-Tools` v7.0.

## Goals

- Existing plugins should continue to work with minimal or no code changes.
- New development should move to non-`System.Drawing`-centric APIs over time.
- Migration should be incremental and predictable.

## What changed in v7.0

### Target Frameworks
- **Before**: `netstandard2.0` + `net8.0`
- **After**: `net48` + `net8.0` + `net9.0`

### Internal graphics abstraction
All image encode/decode operations now route through an internal `IImageCodec` abstraction backed by `System.Drawing`. This is transparent to plugins -- existing code continues to work without changes.

### New API: `Image.ToPngByteArray()`
Encodes an image to PNG bytes via the codec abstraction. Replaces the old `Image.ToByteArray()` which used BMP format.

### Deprecated API: `Image.ToByteArray()`
Marked `[Obsolete]`. Encodes as BMP which will not be supported in future backend swaps. Use `Image.ToPngByteArray()` instead.

## Upgrade Paths

### Path A: No-Code-Change (recommended first step)

1. Upgrade to StreamDeck-Tools v7.0.
2. Retarget your plugin to `net48` (if using .NET Framework) or `net8.0`/`net9.0`.
3. Build and test -- existing calls like `SetImageAsync(Image, ...)`, `Tools.ImageToBase64`, `Tools.GenerateKeyImage` all work without changes.
4. Fix any `[Obsolete]` warnings (currently only `ToByteArray` -> `ToPngByteArray`).

### Path B: Proactive Modernization

- Move to base64/byte-based APIs for image updates (prefer `SetImageAsync(string base64Image, ...)`).
- Use `Image.ToPngByteArray()` instead of `Image.ToByteArray()`.
- Reduce direct usage of `System.Drawing` in plugin code where possible.

## API Mapping

| Current API | Replacement | Effort | Notes |
| --- | --- | --- | --- |
| `SetImageAsync(Image, ...)` | `SetImageAsync(string base64Image, ...)` | rename + conversion | Convert image once via `Tools.ImageToBase64` and reuse |
| `Tools.ImageToBase64(Image, ...)` | same | no change | Internally adapted, signature unchanged |
| `Tools.Base64StringToImage(string)` | same | no change | Internally adapted, signature unchanged |
| `Tools.FileToBase64(string, ...)` | same | no change | Internally adapted, signature unchanged |
| `Tools.ImageToSHA512(Image)` | same | no change | Internally adapted, signature unchanged |
| `Image.ToByteArray()` | `Image.ToPngByteArray()` | rename | BMP -> PNG. `[Obsolete]` warning guides you |
| `Image.ToBase64(bool)` | same | no change | Delegates to adapted `Tools.ImageToBase64` |
| `Tools.GenerateKeyImage(...)` | same (for now) | no change | Returns `Bitmap` + `Graphics`; future replacement planned |
| `GraphicsTools.*` | same (for now) | no change | System.Drawing signatures retained; future replacement planned |
| `TitleParameters` properties | same (for now) | no change | `Color`, `FontFamily`, `FontStyle` retained; future replacement planned |

## Direct System.Drawing Detection Checklist

Scan your plugin code for direct `System.Drawing` dependencies that may need future migration:

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

- **v7.0** (current): Legacy APIs remain fully supported. Only `Image.ToByteArray()` is marked `[Obsolete]`.
- **v7.x**: Additional `[Obsolete]` warnings will be added to APIs with `System.Drawing` types in signatures, each with clear replacement guidance.
- **v8.0**: Removal candidates evaluated after adoption feedback and compatibility results.

## Compatibility Notes

- Exact pixel parity is not guaranteed for every text/layout edge case across rendering backends.
- Functional parity is the target for common plugin scenarios (resize, encoding, title/image updates).
- Plugins with heavy direct `System.Drawing` usage may require focused migration when the backend swap completes.

## Reference

- [API Inventory](API_INVENTORY.md) -- Complete classification of every public API's System.Drawing exposure.
- [Pre-Existing Issues](PRE_EXISTING_ISSUES.md) -- Known issues unrelated to the migration.
