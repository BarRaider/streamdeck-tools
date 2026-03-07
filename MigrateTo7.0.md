# Migrating to StreamDeck-Tools v7.0

This guide helps you upgrade your Stream Deck plugin from StreamDeck-Tools v6.x to v7.0.

> **AI-assisted migration:** This document is designed to work well with AI coding assistants (Cursor, Copilot, etc.). Point your AI at this file and ask it to migrate your plugin -- the tables and recipes below give it everything it needs.

## What's New in v7.0

- **Cross-platform support** -- plugins can now run on both Windows and macOS
- **SkiaSharp graphics** -- new cross-platform drawing APIs alongside the existing System.Drawing ones
- **.NET 10 support** -- library targets `netstandard2.0`, `net48`, `net8.0`, and `net10.0`
- **Self-contained deployment** -- ship your plugin with the runtime included, no user install required

## Do I Need to Change My Code?

Maybe not! Here's a quick check:

```
Does your plugin use System.Drawing directly in its own code?
  │
  ├── NO  → Just upgrade the NuGet package. Build. Done.
  │         You'll see [Obsolete] warnings -- these are informational.
  │         Everything still works on Windows.
  │
  └── YES → Does it use GraphicsPath, LockBits, RotateTransform, or SetPixel?
              │
              ├── NO  → Straightforward rename using the tables below.
              │
              └── YES → Rewrite those sections using SkiaSharp directly.
```

Most plugins (roughly half) fall into the "just upgrade" category.

## Step 1: Upgrade the NuGet Package

```
Update-Package StreamDeck-Tools
```

Or set the version manually in your `.csproj`:

```xml
<PackageReference Include="StreamDeck-Tools" Version="7.0.0-beta.2" />
```

Build your project. Everything should compile. You'll see `[Obsolete]` warnings on System.Drawing methods -- these point you to the SkiaSharp replacements.

## Step 2: Add SkiaSharp Usings

In files where you do image rendering, add:

```csharp
using SkiaSharp;
```

The `BarRaider.SdTools` namespace already contains `SkiaTools`, `SkiaGraphicsTools`, and `SkiaExtensionMethods`.

## Step 3: Replace System.Drawing Calls

### Class Mapping

| Legacy Class | Replacement |
|---|---|
| `Tools` (image/font methods) | `SkiaTools` |
| `GraphicsTools` | `SkiaGraphicsTools` |
| `ExtensionMethods` (on `Image`/`Graphics`) | `SkiaExtensionMethods` (on `SKBitmap`/`SKCanvas`) |

### Type Mapping

| System.Drawing | SkiaSharp | Notes |
|---|---|---|
| `Image` / `Bitmap` | `SKBitmap` | Disposable |
| `Graphics` | `SKCanvas` | Created from `SKBitmap` |
| `Color` | `SKColor` | Struct, no disposal |
| `Font` | `SKFont` | Disposable |
| `FontFamily` | `SKTypeface` | `SKTypeface.FromFamilyName(...)` |
| `FontStyle` | `SKFontStyle` | `SKFontStyle.Bold`, `.Normal`, etc. |
| `SolidBrush` | `SKPaint` | `Style = SKPaintStyle.Fill` |
| `Pen` | `SKPaint` | `Style = SKPaintStyle.Stroke` |
| `PointF` | `SKPoint` | |
| `Rectangle` / `RectangleF` | `SKRect` / `SKRectI` | |
| `ColorTranslator.FromHtml(...)` | `SkiaTools.ColorFromHex(...)` | |
| `Image.FromFile(...)` | `SkiaTools.LoadImage(path)` | |

### Common Patterns

| Before (System.Drawing) | After (SkiaSharp) |
|---|---|
| `new SolidBrush(color)` | `new SKPaint { Color = skColor, Style = SKPaintStyle.Fill }` |
| `ColorTranslator.FromHtml("#FF0000")` | `SkiaTools.ColorFromHex("#FF0000")` |
| `new Font("Arial", 12)` | `SkiaTools.CreateFont("Arial", 12)` |
| `Image.FromFile("icon.png")` | `SkiaTools.LoadImage("icon.png")` |
| `graphics.DrawString(text, font, brush, point)` | `canvas.DrawText(text, x, y, font, paint)` |
| `graphics.FillRectangle(brush, rect)` | `canvas.DrawRect(rect, paint)` |

### Method Migration

| Legacy | Replacement |
|---|---|
| `Tools.GenerateKeyImage(type, out Graphics)` | `SkiaTools.GenerateKeyImage(type, out SKCanvas)` |
| `Tools.GenerateGenericKeyImage(out Graphics)` | `SkiaTools.GenerateGenericKeyImage(out SKCanvas)` |
| `Tools.ImageToBase64(Image, bool)` | `SkiaTools.ImageToBase64(SKBitmap, bool)` |
| `Tools.Base64StringToImage(string)` → `Image` | `SkiaTools.Base64StringToImage(string)` → `SKBitmap` |
| `Tools.FileToBase64(string, bool)` | `SkiaTools.FileToBase64(string, bool)` |
| `Tools.LoadImage(string/Stream)` → `Image` | `SkiaTools.LoadImage(string/Stream)` → `SKBitmap` |
| `Tools.CreateFont(...)` → `Font` | `SkiaTools.CreateFont(name, size, SKFontStyle?)` → `SKFont` |
| `GraphicsTools.ResizeImage(Image, w, h)` | `SkiaGraphicsTools.ResizeImage(SKBitmap, w, h)` |
| `GraphicsTools.CreateOpacityImage(Image, f)` | `SkiaGraphicsTools.CreateOpacityImage(SKBitmap, f)` |
| `Connection.SetImageAsync(Image)` | `Connection.SetImageAsync(SKBitmap)` |
| `graphics.AddTextPath(tp, h, w, text)` | `canvas.AddTextPath(tp, h, w, text)` |
| `graphics.GetTextCenter(text, w, font)` | `canvas.GetTextCenter(text, w, skFont)` |
| `text.SplitToFitKey(tp, ...)` | `text.SplitToFitKey(tp, skFont, ...)` |

> **Coordinate difference:** `Graphics.DrawString` positions text from the top-left corner. `SKCanvas.DrawText` uses the text **baseline**. You may need to adjust Y coordinates when migrating (add the font ascent to the Y value).

> **SKFont from TitleParameters:** Some methods now require an `SKFont`. Create one from `TitleParameters` like this: `new SKFont(titleParameters.TitleTypeface, (float)titleParameters.FontSizeInPixelsScaledToDefaultImage)`. Remember to dispose it.

## Code Recipes

### Key Image Rendering

**Before:**
```csharp
using (Image image = Tools.GenerateGenericKeyImage(out Graphics graphics))
{
    graphics.FillRectangle(new SolidBrush(Color.White), 0, 0, image.Width, image.Height);
    graphics.DrawString("Hello", new Font("Arial", 20), new SolidBrush(Color.Black), new PointF(10, 50));
    graphics.Dispose();
    await Connection.SetImageAsync(image);
}
```

**After:**
```csharp
using (SKBitmap image = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas))
{
    canvas.Clear(SKColors.White);
    using (var font = SkiaTools.CreateFont("Arial", 20))
    using (var paint = new SKPaint { Color = SKColors.Black, IsAntialias = true })
    {
        canvas.DrawText("Hello", 10, 50, font, paint);
    }
    canvas.Dispose();
    await Connection.SetImageAsync(image);
}
```

### Title Text with TitleParameters

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

### Color Parsing

**Before:** `Color c = ColorTranslator.FromHtml("#FF0000");`

**After:** `SKColor c = SkiaTools.ColorFromHex("#FF0000");`

### Image Loading

**Before:**
```csharp
Image img = Tools.LoadImage("icon.png");
string base64 = Tools.ImageToBase64(img, true);
await Connection.SetImageAsync(base64);
```

**After:**
```csharp
using (SKBitmap img = SkiaTools.LoadImage("icon.png"))
{
    string base64 = SkiaTools.ImageToBase64(img, true);
    await Connection.SetImageAsync(base64);
}
```

### Font Creation

**Before:** `Font font = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Pixel);`

**After:**
```csharp
using (SKFont font = SkiaTools.CreateFont("Arial", 14, SKFontStyle.Bold))
{
    // use font for drawing
}
```

## Going Cross-Platform (Optional)

If you want your plugin to run on both Windows and macOS:

### 1. Target .NET 10 with Self-Contained Deployment

```xml
<PropertyGroup>
  <TargetFramework>net10.0</TargetFramework>
  <SelfContained>true</SelfContained>
  <PublishTrimmed>true</PublishTrimmed>
  <TrimMode>partial</TrimMode>
  <RuntimeIdentifiers>win-x64;osx-x64</RuntimeIdentifiers>
</PropertyGroup>
```

`osx-x64` is recommended for macOS -- it runs natively on Intel and via Rosetta 2 on Apple Silicon, covering all Mac users with a single binary.

### 2. Update manifest.json

```json
{
  "CodePath": "com.your.plugin",
  "CodePathWin": "win-x64/com.your.plugin.exe",
  "CodePathMac": "osx-x64/com.your.plugin",
  "OS": [
    { "Platform": "windows", "MinimumVersion": "10" },
    { "Platform": "mac", "MinimumVersion": "13" }
  ]
}
```

### 3. Publish for Both Platforms

```powershell
dotnet publish -c Release -r win-x64
dotnet publish -c Release -r osx-x64
```

Assemble both outputs into your `.sdPlugin` folder with `win-x64/` and `osx-x64/` subdirectories, alongside shared assets (manifest, images, Property Inspector).

### Known Gotcha: System.Management and Trimming

`System.Management` (WMI) does not work in trimmed .NET 10 builds due to COM interop incompatibility. If your plugin or any dependency uses WMI, replace those calls with alternatives (e.g., Windows Registry reads). This only affects trimmed self-contained builds.

## Deprecation Timeline

| Release | What Happens |
|---|---|
| **v7.0** (current) | System.Drawing APIs marked `[Obsolete]` with guidance. Everything still works on Windows. SkiaSharp APIs available in parallel. |
| **v7.x** | Stability period. No removals. |
| **v8.0** (future) | System.Drawing APIs evaluated for removal based on adoption. |

## Compatibility Notes

- System.Drawing APIs continue to work on **Windows** across all TFMs.
- For **macOS/Linux**, you must use the SkiaSharp APIs. System.Drawing throws `PlatformNotSupportedException` on non-Windows.
- `TitleParameters` is unchanged. The new SkiaSharp properties (`TitleSKColor`, `TitleTypeface`) are additive.
- SkiaSharp 3.x is MIT-licensed. No licensing impact.

## Complete Example: Cross-Platform Plugin Action

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

            using (var font = SkiaTools.CreateFont("Arial", 18, SKFontStyle.Bold))
            using (var paint = new SKPaint { Color = SKColors.White, IsAntialias = true })
            {
                float x = canvas.GetTextCenter("Pressed!", image.Width, font);
                canvas.DrawText("Pressed!", x, image.Height / 2f, font, paint);
            }

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
