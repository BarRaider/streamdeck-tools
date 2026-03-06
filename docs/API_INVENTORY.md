# System.Drawing Public API Inventory

Complete inventory of every public API in `barraider-sdtools` that exposes `System.Drawing` types.
Each entry is classified as: **ADAPTED**, **KEPT** (System.Drawing retained, future [Obsolete]), or **HIGH-RISK**.

---

## Summary

| Classification | Count | Description |
| --- | --- | --- |
| ADAPTED | 8 | Routed through internal codec abstraction (Phase 1 + 2) |
| KEPT | 9 | System.Drawing types retained in signatures; will be marked [Obsolete] in a future release |
| HIGH-RISK | 12 | Deep entanglement with System.Drawing types in public signatures |

**Files with System.Drawing exposure:**
- `Tools/Tools.cs` (5 APIs)
- `Tools/GraphicsTools.cs` (7 APIs)
- `Tools/ExtensionMethods.cs` (10 APIs)
- `Backend/SDConnection.cs` (1 API)
- `Backend/ISDConnection.cs` (1 API)
- `Wrappers/TitleParameters.cs` (3 properties + 2 constructors)

---

## ADAPTED (Phase 1 + Phase 2 -- complete)

### Tools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 1 | `Tools.ImageToBase64` | `public static string ImageToBase64(Image image, bool addHeaderPrefix)` | `Image` (param) | Routes through `ImageCodecProvider.Instance.EncodeToPngBytes` |
| 2 | `Tools.Base64StringToImage` | `public static Image Base64StringToImage(string base64String)` | `Image` (return) | Routes through `ImageCodecProvider.Instance.DecodeFromBytes` |
| 3 | `Tools.ImageToSHA512` | `public static string ImageToSHA512(Image image)` | `Image` (param) | Routes through `ImageCodecProvider.Instance.EncodeToPngBytes` |
| 4 | `Tools.FileToBase64` | `public static string FileToBase64(string fileName, bool addHeaderPrefix)` | `Image` (internal only) | Routes through `ImageCodecProvider.Instance.DecodeFromFile`. No public SD exposure. |

### SDConnection.cs / ISDConnection.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 5 | `SDConnection.SetImageAsync` | `public async Task SetImageAsync(Image image, int? state, bool forceSendToStreamdeck)` | `Image` (param) | Converts to base64 via `Tools.ImageToBase64`, then calls string overload |
| 6 | `ISDConnection.SetImageAsync` | `Task SetImageAsync(Image image, int? state, bool forceSendToStreamdeck)` | `Image` (param) | Interface declaration matching #5 |

### ExtensionMethods.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 7 | `Image.ToBase64` | `public static string ToBase64(this Image image, bool addHeaderPrefix)` | `Image` (this) | Delegates to adapted `Tools.ImageToBase64` |
| 8 | `Image.ToPngByteArray` | `public static byte[] ToPngByteArray(this Image image)` | `Image` (this) | **NEW** -- routes through `ImageCodecProvider.Instance.EncodeToPngBytes` |

---

## KEPT (System.Drawing signatures retained -- future [Obsolete])

### Tools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 9 | `Tools.GenerateKeyImage` | `public static Bitmap GenerateKeyImage(DeviceType streamDeckType, out Graphics graphics)` | `Bitmap` (return), `Graphics` (out) | Core drawing entry point for plugins. Kept as-is. |
| 10 | `Tools.GenerateGenericKeyImage` | `public static Bitmap GenerateGenericKeyImage(out Graphics graphics)` | `Bitmap` (return), `Graphics` (out) | Delegates to #9. |

### GraphicsTools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 11 | `GraphicsTools.ResizeImage` | `public static Image ResizeImage(Image original, int newWidth, int newHeight)` | `Image` (param + return) | Kept as-is; System.Drawing types in signature. |
| 12 | `GraphicsTools.ExtractRectangle` | `public static Bitmap ExtractRectangle(Image image, int startX, int startY, int width, int height)` | `Image` (param), `Bitmap` (return) | Kept as-is. |
| 13 | `GraphicsTools.CreateOpacityImage` | `public static Image CreateOpacityImage(Image image, float opacity)` | `Image` (param + return) | Kept as-is. |
| 14 | `GraphicsTools.DrawMultiLinedText` | `public static Image[] DrawMultiLinedText(...)` | `Font`, `Color` (params), `Image[]` (return), `PointF` | Kept as-is. |
| 15 | `GraphicsTools.WrapStringToFitImage` | `public static string WrapStringToFitImage(...)` | Uses `Font`, `Bitmap`, `Graphics` internally | Kept as-is. |

### ExtensionMethods.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 16 | `Image.ToByteArray` | `public static byte[] ToByteArray(this Image image)` | `Image` (this) | Marked `[Obsolete]` -- use `ToPngByteArray` instead. BMP encoding kept for backward compatibility. |
| 17 | `string.SplitToFitKey` | `public static string SplitToFitKey(this string str, TitleParameters titleParameters, ...)` | Uses `Font`, `Bitmap`, `Graphics` internally | Kept as-is. |

---

## HIGH-RISK (deep System.Drawing entanglement in public surface)

### GraphicsTools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 18 | `GraphicsTools.ColorFromHex` | `public static Color ColorFromHex(string hexColor)` | `Color` (return) | Uses `ColorTranslator.FromHtml`. |
| 19 | `GraphicsTools.GenerateColorShades` | `public static Color GenerateColorShades(string initialColor, int currentShade, int totalAmountOfShades)` | `Color` (return) | Uses `Color.FromArgb`. |

### ExtensionMethods.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 20 | `Color.ToHex` | `public static string ToHex(this Color color)` | `Color` (this) | Extension on `Color`. |
| 21 | `Brush.ToHex` | `public static string ToHex(this Brush brush)` | `Brush` (this), `SolidBrush` | Extension on `Brush`. |
| 22 | `Graphics.DrawAndMeasureString` | `public static float DrawAndMeasureString(this Graphics graphics, string text, Font font, Brush brush, PointF position)` | `Graphics`, `Font`, `Brush`, `PointF` | Extension on `Graphics`. |
| 23 | `Graphics.GetTextCenter` | `public static float GetTextCenter(this Graphics graphics, string text, int imageWidth, Font font, out bool textFitsImage, int minIndentation)` | `Graphics`, `Font` | Extension on `Graphics`. Two overloads. |
| 24 | `Graphics.GetFontSizeWhereTextFitsImage` | `public static float GetFontSizeWhereTextFitsImage(this Graphics graphics, string text, int imageWidth, Font font, int minimalFontSize)` | `Graphics`, `Font` | Extension on `Graphics`. |
| 25 | `Graphics.AddTextPath` | `public static void AddTextPath(this Graphics graphics, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, Color strokeColor, float strokeThickness, int pixelsAlignment)` | `Graphics`, `Color`, `TitleParameters` | Extension on `Graphics`. Two overloads. Uses `GraphicsPath`, `Pen`, `SolidBrush`. |

### TitleParameters.cs (Wrappers)

| # | API | Type | Notes |
| --- | --- | --- | --- |
| 26 | `TitleParameters.TitleColor` | `Color` property | Public property, deserialized from JSON. |
| 27 | `TitleParameters.FontFamily` | `FontFamily` property | Public property, deserialized from JSON. |
| 28 | `TitleParameters.FontStyle` | `FontStyle` property (enum) | Public property, deserialized from JSON. |
| 29 | `TitleParameters(FontFamily, FontStyle, double, Color, bool, TitleVerticalAlignment)` | Constructor | Direct SD types in parameters. |

---

## Dependency Graph

```
Plugin Code
    |
    +-- SDConnection.SetImageAsync(Image) ........... [ADAPTED]
    +-- Tools.ImageToBase64(Image) .................. [ADAPTED]
    +-- Tools.Base64StringToImage(string) ........... [ADAPTED]
    +-- Tools.FileToBase64(string) .................. [ADAPTED]
    +-- Image.ToPngByteArray() ...................... [ADAPTED - NEW]
    +-- Image.ToByteArray() ......................... [OBSOLETE -> ToPngByteArray]
    +-- Tools.GenerateKeyImage(DeviceType, out Graphics) ... [KEPT - returns Bitmap+Graphics]
    |       |
    |       +-- Plugin draws on Graphics, then calls SetImageAsync(Image)
    |
    +-- GraphicsTools.ResizeImage(Image) ............ [KEPT]
    +-- GraphicsTools.CreateOpacityImage(Image) ..... [KEPT]
    +-- GraphicsTools.DrawMultiLinedText(...) ........ [KEPT]
    +-- ExtensionMethods on Graphics ................ [HIGH-RISK - direct SD types]
    +-- TitleParameters (Color/FontFamily/FontStyle)  [HIGH-RISK - data class]
```

---

## Future Deprecation Priority

When the time comes to mark KEPT APIs as `[Obsolete]` and provide parallel replacements:

1. **`Tools.GenerateKeyImage` / `GenerateGenericKeyImage`** -- Most commonly used plugin entry point for drawing.
2. **`GraphicsTools.ResizeImage` / `ExtractRectangle` / `CreateOpacityImage`** -- Image manipulation utilities, frequently used.
3. **`GraphicsTools.DrawMultiLinedText`** -- Text rendering, depends on Font/Color.
4. **`TitleParameters`** -- Core data class. Needs careful deprecation strategy since plugins read properties directly.
5. **Extension methods on `Graphics`** -- Lowest priority; advanced drawing helpers tightly coupled to `System.Drawing.Graphics`.
