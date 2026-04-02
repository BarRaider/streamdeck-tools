# StreamDeck-Tools v7.0 Migration Guide

Migration guide for upgrading from `StreamDeck-Tools` v6.x to v7.0, including the new cross-platform SkiaSharp API surface.

---

## What Changed in v7.0

### Target Frameworks

| Before (v6.x) | After (v7.0) |
|---|---|
| `netstandard2.0` + `net48` | `netstandard2.0` + `net48` + `net8.0` + `net10.0` |

The library retains `netstandard2.0` for broad compatibility and `net48` for .NET Framework consumers. `net8.0` (LTS) and `net10.0` (current) are added for modern .NET. `net9.0` is not included (EOL May 2026).

### New Dependency: SkiaSharp 3.119.2

The library now ships with [SkiaSharp](https://github.com/mono/SkiaSharp) (MIT license) as a cross-platform graphics backend. SkiaSharp works on Windows, macOS, and Linux without GDI+.

### Dual API Surface

Every public method that accepted or returned `System.Drawing` types now has a SkiaSharp equivalent in a parallel class:

| Legacy Class | SkiaSharp Replacement |
|---|---|
| `Tools` | `SkiaTools` |
| `GraphicsTools` | `SkiaGraphicsTools` |
| `ExtensionMethods` (on `Image`/`Graphics`) | `SkiaExtensionMethods` (on `SKBitmap`/`SKCanvas`) |

### System.Drawing APIs Marked `[Obsolete]`

All public methods using `System.Drawing` types are now marked `[Obsolete]` with messages pointing to their SkiaSharp replacements. They still compile and work on Windows but will **not** work on macOS/Linux with .NET 8+.

### New `SetImageAsync` Overloads

`ISDConnection` and `SDConnection` now support:

| Method | Status | Platform |
|---|---|---|
| `SetImageAsync(SKBitmap, ...)` | **New** | Cross-platform |
| `SetImageAsync(byte[], ...)` | **New** | Cross-platform (raw PNG bytes) |
| `SetImageAsync(string, ...)` | Unchanged | Cross-platform (base64) |
| `SetImageAsync(Image, ...)` | `[Obsolete]` | Windows-only |

### TitleParameters Changes

`TitleParameters` retains its existing constructors and properties unchanged. New read-only SkiaSharp properties are added:

| Property / Method | Type | Description |
|---|---|---|
| `FontFamilyName` | `string` | Cross-platform font family name. Replaces `FontFamily.Name`. |
| `TitleSKColor` | `SKColor` | Derived from `TitleColor`. |
| `TitleTypeface` | `SKTypeface` | Derived from `FontFamilyName` + `FontStyle`. Cached. |
| `FontStyleToSKFontStyle()` | `SKFontStyle` | Converts `System.Drawing.FontStyle` to `SKFontStyle`. |

> **Breaking change (macOS):** `TitleParameters.FontFamily` (the `System.Drawing.FontFamily` property) is `[Obsolete]` and **throws `PlatformNotSupportedException`** on non-Windows. This is a hard crash, not just a warning. Any code that accesses `.FontFamily` (including in `OnTitleParametersDidChange` handlers) must switch to `.FontFamilyName` or `.TitleTypeface`.

### DrawTextLine

New extension method `SKCanvas.DrawTextLine(string, SKFont, SKPaint, SKPoint)` draws text treating Y as the **top of the text** (matching `System.Drawing.Graphics.DrawString` behavior) and returns the Y position for the next line.

Use `DrawTextLine` instead of raw `SKCanvas.DrawText` when stacking multiple lines -- `DrawText` uses baseline Y which leads to positioning bugs.

`DrawAndMeasureString` is also available on `SKCanvas` and delegates to `DrawTextLine`.

### PluginBase Deprecation

`PluginBase` is `[Obsolete]`. Use `KeypadBase` (keys only), `EncoderBase` (dials only), or `KeyAndEncoderBase` (both).

---

## Type Mapping Reference

| System.Drawing Type | SkiaSharp Type | Notes |
|---|---|---|
| `Image` / `Bitmap` | `SKBitmap` | `IDisposable`. Use `using` blocks. |
| `Graphics` | `SKCanvas` | Created from `SKBitmap`. `IDisposable`. |
| `Color` | `SKColor` | Struct, no disposal needed. |
| `Font` | `SKFont` | `IDisposable`. Holds typeface + size. |
| `FontFamily` | `SKTypeface` | Use `SKTypeface.FromFamilyName(...)`. |
| `FontStyle` | `SKFontStyle` | `SKFontStyle.Bold`, `SKFontStyle.Normal`, etc. |
| `SolidBrush` | `SKPaint` | Set `Style = SKPaintStyle.Fill` and `Color`. |
| `Pen` | `SKPaint` | Set `Style = SKPaintStyle.Stroke` and `StrokeWidth`. |
| `PointF` | `SKPoint` | |
| `Rectangle` / `RectangleF` | `SKRect` / `SKRectI` | |
| `ColorTranslator.FromHtml(...)` | `SkiaTools.ColorFromHex(...)` | Or `SKColor.TryParse(...)` directly. |
| `Image.FromFile(...)` | `SkiaTools.LoadImage(path)` | |
| `Image.FromStream(...)` | `SkiaTools.LoadImage(stream)` | |

---

## Method-by-Method Migration

### Tools -> SkiaTools

| Legacy (`Tools.*`) | Replacement (`SkiaTools.*`) | Notes |
|---|---|---|
| `GenerateKeyImage(DeviceType, out Graphics)` | `GenerateKeyImage(DeviceType, out SKCanvas)` | Returns `SKBitmap` + `SKCanvas` |
| `GenerateGenericKeyImage(out Graphics)` | `GenerateGenericKeyImage(out SKCanvas)` | Returns `SKBitmap` + `SKCanvas` |
| `ImageToBase64(Image, bool)` | `ImageToBase64(SKBitmap, bool)` | |
| `Base64StringToImage(string)` returns `Image` | `Base64StringToImage(string)` returns `SKBitmap` | |
| `FileToBase64(string, bool)` | `FileToBase64(string, bool)` | Identical signature |
| `LoadImage(string)` returns `Image` | `LoadImage(string)` returns `SKBitmap` | |
| `LoadImage(Stream)` returns `Image` | `LoadImage(Stream)` returns `SKBitmap` | |
| `ImageToSHA512(Image)` | `ImageToSHA512(SKBitmap)` | |
| `CreateFont(...)` returns `Font` | `CreateFont(string, float, SKFontStyle?)` returns `SKFont` | Style defaults to Normal |
| *(no equivalent)* | `ColorFromHex(string)` returns `SKColor` | New helper |

### GraphicsTools -> SkiaGraphicsTools

| Legacy (`GraphicsTools.*`) | Replacement (`SkiaGraphicsTools.*`) | Notes |
|---|---|---|
| `ColorFromHex(string)` returns `Color` | `ColorFromHex(string)` returns `SKColor` | |
| `GenerateColorShades(string, int, int)` returns `Color` | `GenerateColorShades(string, int, int)` returns `SKColor` | |
| `ResizeImage(Image, int, int)` | `ResizeImage(SKBitmap, int, int)` | Returns `SKBitmap` |
| `ExtractRectangle(Image, ...)` | `ExtractRectangle(SKBitmap, ...)` | Returns `SKBitmap` |
| `CreateOpacityImage(Image, float)` | `CreateOpacityImage(SKBitmap, float)` | Returns `SKBitmap` |
| `DrawMultiLinedText(...)` with `Font`, `Color`, `PointF` | `DrawMultiLinedText(...)` with `SKFont`, `SKColor`, `SKPoint` | Returns `SKBitmap[]` |
| `WrapStringToFitImage(string, TitleParameters, leftPad, rightPad, imageWidth)` | `WrapStringToFitImage(string, SKFont, imageWidth, leftPad, rightPad)` | Takes `SKFont` instead of `TitleParameters`. **Parameter order changed.** |

### ExtensionMethods -> SkiaExtensionMethods

| Legacy | Replacement | Notes |
|---|---|---|
| `Image.ToPngByteArray()` | `SKBitmap.ToPngByteArray()` | |
| `Image.ToBase64(bool)` | `SKBitmap.ToBase64(bool)` | |
| `Image.ToByteArray()` (BMP) | *(removed)* | Use `ToPngByteArray()` instead |
| `Graphics.DrawAndMeasureString(string, Font, Brush, PointF)` | `SKCanvas.DrawTextLine(string, SKFont, SKPaint, SKPoint)` | **Preferred.** Y = top of text. Returns next-line Y. |
| `Graphics.GetTextCenter(...)` | `SKCanvas.GetTextCenter(...)` | Same semantics |
| `Graphics.GetFontSizeWhereTextFitsImage(...)` | `SKCanvas.GetFontSizeWhereTextFitsImage(...)` | Same semantics |
| `Graphics.AddTextPath(TitleParameters, ...)` | `SKCanvas.AddTextPath(TitleParameters, ...)` | Uses `TitleSKColor` and `TitleTypeface` internally |
| `string.SplitToFitKey(TitleParameters, ...)` | `string.SplitToFitKey(TitleParameters, SKFont, ...)` | **Requires explicit `SKFont` parameter** |
| `Color.ToHex()` | Use `SKColor` directly | |
| `Brush.ToHex()` | Use `SKPaint.Color` directly | |

### Connection

| Legacy | Replacement | Notes |
|---|---|---|
| `SetImageAsync(Image, ...)` | `SetImageAsync(SKBitmap, ...)` | Direct replacement |
| | `SetImageAsync(byte[], ...)` | Alternative: pass raw PNG bytes |

---

## Getting an `SKFont` from `TitleParameters`

Several methods (`WrapStringToFitImage`, `SplitToFitKey`, `GetTextCenter`, etc.) now require an `SKFont`. Create one from `TitleParameters`:

```csharp
using var font = new SKFont(titleParameters.TitleTypeface, (float)titleParameters.FontSizeInPixelsScaledToDefaultImage);
```

The `SKFont` is `IDisposable` -- use a `using` statement or dispose it manually.

---

## Disposal Patterns

`SKBitmap`, `SKCanvas`, `SKFont`, and `SKPaint` are all `IDisposable`. Wrap them in `using` statements.

`SetImageAsync(SKBitmap)` encodes the bitmap to PNG bytes synchronously before the async send, so it is safe to dispose the bitmap immediately after the `await`:

```csharp
using (SKBitmap image = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas))
{
    // ... draw on canvas ...
    canvas.Dispose();
    await Connection.SetImageAsync(image);
}
// image is disposed here -- safe, encoding already completed
```

For bitmaps stored as fields (e.g., cached key images), dispose them in your action's `Dispose()` method.

---

## Migration Recipes

### Recipe 1: Basic Key Image Rendering

**Before (System.Drawing):**
```csharp
using (Image image = Tools.GenerateGenericKeyImage(out Graphics graphics))
{
    graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, image.Width, image.Height);
    graphics.DrawString("Hello", new Font("Arial", 20), new SolidBrush(Color.Black), new PointF(10, 50));
    graphics.Dispose();
    await Connection.SetImageAsync(image);
}
```

**After (SkiaSharp):**
```csharp
using (SKBitmap image = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas))
{
    canvas.Clear(SKColors.White);
    using var font = SkiaTools.CreateFont("Arial", 20);
    using var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true };
    canvas.DrawTextLine("Hello", font, paint, new SKPoint(10, 50));
    canvas.Dispose();
    await Connection.SetImageAsync(image);
}
```

### Recipe 2: Title Text with TitleParameters

**Before:**
```csharp
TitleParameters tp = new TitleParameters(new FontFamily("Arial"), FontStyle.Bold, 20, Color.White, true, TitleVerticalAlignment.Middle);
using (Image image = Tools.GenerateGenericKeyImage(out Graphics graphics))
{
    graphics.AddTextPath(tp, image.Height, image.Width, "My Title");
    graphics.Dispose();
    await Connection.SetImageAsync(image);
}
```

**After:**
```csharp
TitleParameters tp = new TitleParameters(new FontFamily("Arial"), FontStyle.Bold, 20, Color.White, true, TitleVerticalAlignment.Middle);
using (SKBitmap image = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas))
{
    canvas.AddTextPath(tp, image.Height, image.Width, "My Title");
    canvas.Dispose();
    await Connection.SetImageAsync(image);
}
```

> `AddTextPath` on `SKCanvas` uses `TitleSKColor` and `TitleTypeface` internally -- no changes needed beyond switching from `Graphics` to `SKCanvas`.

### Recipe 3: Color Parsing

**Before:**
```csharp
Color c = ColorTranslator.FromHtml("#FF0000");
// or
Color c = GraphicsTools.ColorFromHex("#FF0000");
```

**After:**
```csharp
SKColor c = SkiaTools.ColorFromHex("#FF0000");
// or
SKColor c = SkiaGraphicsTools.ColorFromHex("#FF0000");
// or
SKColor.TryParse("#FF0000", out SKColor c);
```

### Recipe 4: Image Loading and Base64

**Before:**
```csharp
Image img = Tools.LoadImage("icon.png");
string base64 = Tools.ImageToBase64(img, true);
await Connection.SetImageAsync(base64);
```

**After:**
```csharp
using SKBitmap img = SkiaTools.LoadImage("icon.png");
string base64 = SkiaTools.ImageToBase64(img, true);
await Connection.SetImageAsync(base64);
```

Or skip the base64 step entirely:
```csharp
using SKBitmap img = SkiaTools.LoadImage("icon.png");
await Connection.SetImageAsync(img);
```

### Recipe 5: Image Resizing

**Before:**
```csharp
Image resized = GraphicsTools.ResizeImage(original, 72, 72);
```

**After:**
```csharp
SKBitmap resized = SkiaGraphicsTools.ResizeImage(original, 72, 72);
```

### Recipe 6: Opacity

**Before:**
```csharp
Image faded = GraphicsTools.CreateOpacityImage(original, 0.5f);
```

**After:**
```csharp
SKBitmap faded = SkiaGraphicsTools.CreateOpacityImage(original, 0.5f);
```

### Recipe 7: Font Creation

**Before:**
```csharp
Font font = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Pixel);
```

**After:**
```csharp
using SKFont font = SkiaTools.CreateFont("Arial", 14, SKFontStyle.Bold);
```

### Recipe 8: Text Centering

**Before:**
```csharp
float x = graphics.GetTextCenter("Hello", imageWidth, font, out bool fits);
```

**After:**
```csharp
float x = canvas.GetTextCenter("Hello", imageWidth, skFont, out bool fits);
```

### Recipe 9: Multi-Line Text Layout with DrawTextLine

Use `DrawTextLine` with layout constants tuned for the default 144x144 key image:

```csharp
const int MARGIN_LEFT = 8;
const int MARGIN_TOP = 3;
const int CONTENT_TOP_OFFSET = 10;

using SKBitmap img = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas);
float y = CONTENT_TOP_OFFSET;

using var font = SkiaTools.CreateFont("Arial", 22, SKFontStyle.Bold);
using var paint = new SKPaint { Color = SKColors.White, IsAntialias = true };

y = canvas.DrawTextLine("Title", font, paint, new SKPoint(MARGIN_LEFT, y)) + MARGIN_TOP;
y = canvas.DrawTextLine("Line 2", font, paint, new SKPoint(MARGIN_LEFT, y)) + MARGIN_TOP;
y = canvas.DrawTextLine("Line 3", font, paint, new SKPoint(MARGIN_LEFT, y)) + MARGIN_TOP;

canvas.Dispose();
await Connection.SetImageAsync(img);
```

Each `DrawTextLine` call returns the Y position for the next line based on `font.Spacing`. Adding `MARGIN_TOP` provides a small gap between lines.

---

## Migration Tiers

Based on analysis of real-world plugins:

### Tier 1: No Direct System.Drawing Usage (~52% of plugins)

Plugins that only use library helpers (`GenerateKeyImage`, `SetImageAsync`, `SplitToFitKey`, etc.) without importing `System.Drawing` in their own code.

**What to do:**
1. Upgrade `StreamDeck-Tools` NuGet to v7.0
2. Build -- you will see `[Obsolete]` warnings. Everything still works.
3. When ready, follow the recipes above to switch to SkiaSharp APIs and eliminate warnings.

### Tier 2: Common System.Drawing Patterns (~22% of plugins)

Plugins that use `SolidBrush`, `ColorTranslator.FromHtml`, `Font`, `Image.FromFile`, or `Graphics.DrawString` in their own code. Migration is mechanical renaming using the mapping tables above.

**What to do:**
1. Upgrade the NuGet package
2. Add `using SkiaSharp;` to files with warnings
3. Replace System.Drawing types with SkiaSharp equivalents using the type mapping and recipes

### Tier 3: Deep GDI+ Usage (~26% of plugins)

Plugins that use GDI+ APIs without library wrappers:

| GDI+ API | SkiaSharp Equivalent |
|---|---|
| `GraphicsPath`, `AddString`, `StringFormat` | `SKPath`, `SKCanvas.DrawTextOnPath` |
| `LockBits`, `BitmapData`, `SetPixel` | `SKBitmap.GetPixels()`, `SKBitmap.SetPixel()` |
| `RotateTransform`, matrix operations | `SKCanvas.RotateDegrees()`, `SKCanvas.SetMatrix()` |
| `Graphics.FromImage` rendering pipelines | `new SKCanvas(bitmap)` |

**What to do:**
1. Upgrade the NuGet package
2. Add a direct `SkiaSharp` NuGet reference to your plugin project
3. Rewrite GDI+ drawing logic using SkiaSharp native APIs

### Quick Triage Flowchart

```
Does your plugin import System.Drawing in its own code?
  |
  +-- NO --> Tier 1: Upgrade the NuGet package. Done.
  |
  +-- YES --> Does it use GraphicsPath, LockBits, RotateTransform, or SetPixel?
                |
                +-- NO --> Tier 2: Mechanical rename using the mapping tables.
                |
                +-- YES --> Tier 3: Rewrite GDI+ logic with SkiaSharp.
```

---

## Deprecation Timeline

| Release | What Happens |
|---|---|
| **v7.0** (current) | All System.Drawing APIs marked `[Obsolete]` with replacement guidance. Everything still compiles and works on Windows. SkiaSharp APIs available in parallel. |
| **v7.x** | Stability and feedback period. No removals. Additional helpers may be added based on community feedback. |
| **v8.0** (future) | System.Drawing APIs evaluated for removal. Removal only after sufficient migration window. |

---

## Compatibility Notes

- All System.Drawing APIs continue to work on **Windows** across all four TFMs.
- For **macOS/Linux** with `net8.0`/`net10.0`, you **must** use the SkiaSharp APIs. System.Drawing throws `PlatformNotSupportedException`.
- `TitleParameters` constructor and properties are **unchanged**. The SkiaSharp properties are purely additive.
- Exact pixel parity between System.Drawing and SkiaSharp text rendering is not guaranteed. Functional parity is the target.
- SkiaSharp 3.119.2 is MIT-licensed. No licensing impact on plugin developers.

---

## Required `using` Directives

When migrating to SkiaSharp APIs, add these to your files:

```csharp
using SkiaSharp;
using BarRaider.SdTools; // SkiaTools, SkiaGraphicsTools, SkiaExtensionMethods
```

---

## Complete Sample: Cross-Platform Plugin Action

```csharp
using BarRaider.SdTools;
using BarRaider.SdTools.Wrappers;
using SkiaSharp;
using System.Threading.Tasks;

[PluginActionId("com.example.myplugin")]
public class MyAction : KeypadBase
{
    public MyAction(ISDConnection connection, InitialPayload payload) : base(connection, payload) { }

    public async override void KeyPressed(KeyPayload payload)
    {
        using (SKBitmap image = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas))
        {
            canvas.Clear(SKColors.DarkBlue);
            using var font = SkiaTools.CreateFont("Arial", 18, SKFontStyle.Bold);
            using var paint = new SKPaint { Color = SKColors.White, IsAntialias = true };
            float x = canvas.GetTextCenter("Pressed!", image.Width, font);
            canvas.DrawTextLine("Pressed!", font, paint, new SKPoint(x, image.Height / 2f - 10));
            canvas.Dispose();
            await Connection.SetImageAsync(image);
        }
    }

    public async override void KeyReleased(KeyPayload payload)
    {
        await Connection.SetDefaultImageAsync();
    }

    public override void OnTick() { }
    public override void Dispose() { }
    public override void ReceivedSettings(ReceivedSettingsPayload payload) { }
    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
}
```
