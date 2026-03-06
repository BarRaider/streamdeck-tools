# Plugin Usage Analysis

Analysis of 27 real-world Stream Deck plugins that depend on `streamdeck-tools`, conducted to validate the v7.0 migration strategy and inform Phase 3 planning.

**Source**: `E:\Projects\DotNet\StreamDeck\All_Plugins.sln` (28 projects, 27 unique plugins)

---

## Summary

| Category | Count | % | Description |
| --- | --- | --- | --- |
| No direct System.Drawing usage | 14 | 52% | Use only library helpers; will "just work" with v7.0 |
| Direct System.Drawing -- LOW complexity | 6 | 22% | Common patterns (SolidBrush, Font, ColorTranslator) |
| Direct System.Drawing -- MEDIUM complexity | 3 | 11% | Multiple custom-drawn actions, transforms |
| Direct System.Drawing -- HIGH complexity | 4 | 15% | Deep GDI+ (GraphicsPath, LockBits, pixel access) |

---

## Migration Tiers

### Tier 1: No Changes Needed (14 plugins)

These plugins solely rely on `streamdeck-tools` library helpers and do not directly interact with `System.Drawing` types in their own code. They will work with v7.0 (and any future backend changes) with only a NuGet package upgrade.

| Plugin | Notes |
| --- | --- |
| streamdeck-voicemeeter | Library helpers only |
| streamdeck-stockticker | Library helpers only |
| streamdeck-shadowplay | Library helpers only |
| streamdeck-soundpad | Library helpers only |
| streamdeck-streamcounter | Library helpers only |
| streamdeck-supermacro | Library helpers only |
| streamdeck-windowsmover | Library helpers only |
| streamdeck-battery | Library helpers only |
| streamdeck-audiometer | Library helpers only |
| streamdeck-speedtest | Library helpers only |
| streamdeck-webcam | Library helpers only |
| streamdeck-TextToSpeech | Library helpers only |
| BarRaiderVirtualDesktop | Library helpers only |
| BarRaiderAudio | Library helpers only |

### Tier 2: Low Effort Migration (6 plugins)

These plugins use some direct `System.Drawing` types, but only in common, easily abstractable patterns. The library can provide helper methods to replace these in a future release.

| Plugin | Direct SD Patterns | Migration Path |
| --- | --- | --- |
| streamdeck-spotify | `SolidBrush`, `ColorTranslator.FromHtml`, `Font` | Replace with library color/font helpers |
| streamdeck-streamtimer | `SolidBrush`, `Font` | Replace with library color/font helpers |
| streamdeck-obstools | `SolidBrush`, `ColorTranslator.FromHtml` | Replace with library color helper |
| streamdeck-textfiletools | `SolidBrush`, `Font`, `Image.FromFile` | Replace with library helpers |
| streamdeck-stopwatch | `SolidBrush`, `Font` | Replace with library color/font helpers |
| streamdeck-minecraft | `Image.FromFile`, `Bitmap` | Replace with library image loading helper |

### Tier 3: High Effort Migration (7 plugins)

These plugins use deep GDI+ APIs that cannot be transparently abstracted by the library. Full migration will require plugin developers to adopt a replacement graphics library (e.g., SkiaSharp) directly.

| Plugin | Complexity | Direct SD Patterns | Notes |
| --- | --- | --- | --- |
| streamdeck-chatpager | HIGH | `GraphicsPath`, `StringFormat`, `AddString`, `SolidBrush`, `Font` | Complex text rendering with paths |
| streamdeck-games | HIGH | Multiple game managers with `Graphics.FromImage`, `Bitmap`, `SolidBrush`, `Pen`, custom drawing | Full custom game rendering |
| streamdeck-screensaver | HIGH | `LockBits`, `BitmapData`, pixel-level access | Direct pixel manipulation |
| streamdeck-advancedlauncher | HIGH | `LockBits`, `BitmapData`, `SetPixel` | Icon extraction and pixel manipulation |
| streamdeck-disco | MEDIUM | Matrix effects, `RotateTransform`, `SolidBrush`, `Bitmap` | Transform-heavy animation |
| streamdeck-wintools | MEDIUM | Multiple custom-drawn actions, `SolidBrush`, `Font`, `Bitmap` | Several action classes with drawing |
| barraider-spotify | MEDIUM | `Image.FromFile`, `Image.FromStream`, image buffering/caching | Image loading and caching patterns |

---

## Common Direct System.Drawing Patterns

Frequency of direct `System.Drawing` type usage across the 13 plugins that use them:

| Pattern | Plugin Count | Library Abstraction Status |
| --- | --- | --- |
| `new SolidBrush(...)` | 9 | Not yet abstracted; candidate for Tier 2 helper |
| `new Bitmap(...)` | 7 | Partially abstracted via `IImageCodec.DecodeFromBytes/File` |
| `new Font(...)` | 7 | Not yet abstracted; candidate for Tier 2 helper |
| `Graphics.FromImage(...)` | 7 | Used via `GenerateKeyImage` pattern in library |
| `ColorTranslator.FromHtml(...)` | 5 | Not yet abstracted; candidate for Tier 2 helper (`GraphicsTools.ColorFromHex` exists but uses `ColorTranslator` internally) |
| `Image.FromFile` / `Image.FromStream` | 5 | Partially abstracted via `IImageCodec.DecodeFromFile` |
| `GraphicsPath` | 2 | Cannot be transparently abstracted |
| `LockBits` / `BitmapData` | 2 | Cannot be transparently abstracted |
| `StringFormat` | 2 | Cannot be transparently abstracted |
| `RotateTransform` | 1 | Cannot be transparently abstracted |

---

## Critical API Findings

### TitleParameters Must Not Change

- **14 plugins** use `TitleParameters` (received from Stream Deck events)
- **3 plugins** construct `TitleParameters` directly (spotify, chatpager x2)
- **4 plugins** read the `TitleColor` property
- **4 plugins** construct `Font` from `TitleParameters.FontFamily` and `FontSizeInPixels`
- Usage is almost entirely **read-only** (store from event, pass to library API)
- The constructor signature `(FontFamily, FontStyle, double, Color, bool, TitleVerticalAlignment)` is part of the public contract and must remain stable

**Conclusion**: `TitleParameters` property types (`Color`, `FontFamily`, `FontStyle`) and constructor must remain unchanged in v7.0. Any future changes require a parallel new type with an adapter.

### GenerateKeyImage Is the Dominant Workflow

The canonical plugin workflow for custom rendering:

1. `Tools.GenerateGenericKeyImage(out Graphics graphics)` -- get `Bitmap` + `Graphics`
2. Draw with `Graphics` (`FillRectangle`, `AddTextPath`, `DrawString`, etc.)
3. `graphics.Dispose()`
4. `await Connection.SetImageAsync(image)` -- send to deck

This is used by nearly all plugins that render custom images. The library controls both the image creation and the sending; the `Graphics` object is the "escape hatch" where plugins perform arbitrary drawing. Any future backend swap must continue to return a valid `System.Drawing.Graphics` on `net48`, since plugins draw directly on it.

---

## Implications for Phase 3

### Validated Decisions

1. **Phase 0-2 work is solid** -- no rollback needed
2. **The codec abstraction** (`IImageCodec`, `ImageCodecProvider`) is correct and complete for encode/decode
3. **The ADAPTED APIs** (#1-#8 in API_INVENTORY.md) are properly routed through the abstraction
4. **The KEPT APIs** (#9-#17 in API_INVENTORY.md) correctly retain System.Drawing signatures
5. **The `[Obsolete]` on `ToByteArray`** is appropriate

### Recommendations

1. **TitleParameters MUST remain unchanged in v7.0** -- 3 plugins construct it, 14 read it. Any change breaks half the ecosystem.
2. **Cross-platform (macOS) is a stretch goal, not a blocker** -- all 27 plugins are Windows-only (`net48`, Stream Deck SDK plugins are Windows executables). Cross-platform is future-facing.
3. **Library should add helpers for common Tier 2 patterns** to ease migration:
   - Color parsing from HTML hex (replacing direct `ColorTranslator.FromHtml` calls)
   - Image loading from file/stream (partially done via `DecodeFromFile`)
   - Font creation helper (from family name, size, style)
4. **Phase 3 should prioritize expanding the abstraction layer** (Option B) over immediately adding a SkiaSharp backend (Option A), as this provides immediate value to the 6 Tier 2 plugins without adding a large dependency.
