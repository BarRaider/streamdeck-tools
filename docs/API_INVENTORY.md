# System.Drawing Public API Inventory

Complete inventory of every public API in `barraider-sdtools` that exposes `System.Drawing` types.
Each entry is classified as: **ADAPTED** (Phase 1), **NEEDS ADAPTER**, or **HIGH-RISK**.

---

## Summary

| Classification | Count | Description |
| --- | --- | --- |
| ADAPTED | 5 | Already routed through internal codec abstraction in Phase 1 |
| NEEDS ADAPTER | 15 | Can be wrapped/adapted without breaking existing callers |
| HIGH-RISK | 10 | Deep entanglement with System.Drawing types in public signatures |

**Files with System.Drawing exposure:**
- `Tools/Tools.cs` (5 APIs)
- `Tools/GraphicsTools.cs` (6 APIs)
- `Tools/ExtensionMethods.cs` (10 APIs)
- `Backend/SDConnection.cs` (1 API)
- `Backend/ISDConnection.cs` (1 API)
- `Wrappers/TitleParameters.cs` (3 properties + 2 constructors)

---

## ADAPTED (Phase 1 -- complete)

### Tools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 1 | `Tools.ImageToBase64` | `public static string ImageToBase64(Image image, bool addHeaderPrefix)` | `Image` (param) | Routes through `ImageCodecProvider.Instance.EncodeToPngBytes` |
| 2 | `Tools.Base64StringToImage` | `public static Image Base64StringToImage(string base64String)` | `Image` (return) | Routes through `ImageCodecProvider.Instance.DecodeFromBytes` |
| 3 | `Tools.ImageToSHA512` | `public static string ImageToSHA512(Image image)` | `Image` (param) | Routes through `ImageCodecProvider.Instance.EncodeToPngBytes` |

### SDConnection.cs / ISDConnection.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 4 | `SDConnection.SetImageAsync` | `public async Task SetImageAsync(Image image, int? state, bool forceSendToStreamdeck)` | `Image` (param) | Converts to base64 via `Tools.ImageToBase64`, then calls string overload |
| 5 | `ISDConnection.SetImageAsync` | `Task SetImageAsync(Image image, int? state, bool forceSendToStreamdeck)` | `Image` (param) | Interface declaration matching #4 |

---

## NEEDS ADAPTER (moderate complexity)

### Tools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 6 | `Tools.FileToBase64` | `public static string FileToBase64(string fileName, bool addHeaderPrefix)` | `Image` (internal only) | Uses `Image.FromFile` internally. Return type is `string` -- no public SD exposure. Low effort to adapt. |
| 7 | `Tools.GenerateKeyImage` | `public static Bitmap GenerateKeyImage(DeviceType streamDeckType, out Graphics graphics)` | `Bitmap` (return), `Graphics` (out) | Core drawing entry point for plugins. |
| 8 | `Tools.GenerateGenericKeyImage` | `public static Bitmap GenerateGenericKeyImage(out Graphics graphics)` | `Bitmap` (return), `Graphics` (out) | Delegates to #7. |

### GraphicsTools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 9 | `GraphicsTools.ResizeImage` | `public static Image ResizeImage(Image original, int newWidth, int newHeight)` | `Image` (param + return) | Uses `Bitmap`, `Graphics`, `InterpolationMode` internally. |
| 10 | `GraphicsTools.ExtractRectangle` | `public static Bitmap ExtractRectangle(Image image, int startX, int startY, int width, int height)` | `Image` (param), `Bitmap` (return) | Uses `Rectangle`, `Bitmap.Clone`. |
| 11 | `GraphicsTools.CreateOpacityImage` | `public static Image CreateOpacityImage(Image image, float opacity)` | `Image` (param + return) | Uses `Bitmap`, `Graphics`, `ColorMatrix`, `ImageAttributes`. |
| 12 | `GraphicsTools.DrawMultiLinedText` | `public static Image[] DrawMultiLinedText(string text, int currentTextPosition, int lettersPerLine, int numberOfLines, Font font, Color backgroundColor, Color textColor, bool expandToNextImage, PointF keyDrawStartingPosition)` | `Font`, `Color` (params), `Image[]` (return), `PointF` | Heavy usage of `Graphics`, `SolidBrush`. |
| 13 | `GraphicsTools.WrapStringToFitImage` | `public static string WrapStringToFitImage(string str, TitleParameters titleParameters, int leftPaddingPixels, int rightPaddingPixels, int imageWidthPixels)` | Uses `Font`, `Bitmap`, `Graphics` internally | Return type is `string` -- no direct SD exposure in signature, but depends on `TitleParameters` (which uses SD types). |

### ExtensionMethods.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 14 | `Image.ToByteArray` | `public static byte[] ToByteArray(this Image image)` | `Image` (this) | Uses `ImageFormat.Bmp` directly. |
| 15 | `Image.ToBase64` | `public static string ToBase64(this Image image, bool addHeaderPrefix)` | `Image` (this) | Delegates to `Tools.ImageToBase64` -- effectively already adapted. |
| 16 | `string.SplitToFitKey` | `public static string SplitToFitKey(this string str, TitleParameters titleParameters, ...)` | Uses `Font`, `Bitmap`, `Graphics` internally | Return type is `string`, but internal SD usage. |

---

## HIGH-RISK (deep System.Drawing entanglement in public surface)

### GraphicsTools.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 17 | `GraphicsTools.ColorFromHex` | `public static Color ColorFromHex(string hexColor)` | `Color` (return) | Uses `ColorTranslator.FromHtml`. |
| 18 | `GraphicsTools.GenerateColorShades` | `public static Color GenerateColorShades(string initialColor, int currentShade, int totalAmountOfShades)` | `Color` (return) | Uses `Color.FromArgb`. |

### ExtensionMethods.cs

| # | API | Signature | System.Drawing types | Notes |
| --- | --- | --- | --- | --- |
| 19 | `Color.ToHex` | `public static string ToHex(this Color color)` | `Color` (this) | Extension on `Color`. |
| 20 | `Brush.ToHex` | `public static string ToHex(this Brush brush)` | `Brush` (this), `SolidBrush` | Extension on `Brush`. |
| 21 | `Graphics.DrawAndMeasureString` | `public static float DrawAndMeasureString(this Graphics graphics, string text, Font font, Brush brush, PointF position)` | `Graphics`, `Font`, `Brush`, `PointF` | Extension on `Graphics`. |
| 22 | `Graphics.GetTextCenter` | `public static float GetTextCenter(this Graphics graphics, string text, int imageWidth, Font font, out bool textFitsImage, int minIndentation)` | `Graphics`, `Font` | Extension on `Graphics`. Two overloads. |
| 23 | `Graphics.GetFontSizeWhereTextFitsImage` | `public static float GetFontSizeWhereTextFitsImage(this Graphics graphics, string text, int imageWidth, Font font, int minimalFontSize)` | `Graphics`, `Font` | Extension on `Graphics`. |
| 24 | `Graphics.AddTextPath` | `public static void AddTextPath(this Graphics graphics, TitleParameters titleParameters, int imageHeight, int imageWidth, string text, Color strokeColor, float strokeThickness, int pixelsAlignment)` | `Graphics`, `Color`, `TitleParameters` | Extension on `Graphics`. Two overloads. Uses `GraphicsPath`, `Pen`, `SolidBrush`. |

### TitleParameters.cs (Wrappers)

| # | API | Type | Notes |
| --- | --- | --- | --- |
| 25 | `TitleParameters.TitleColor` | `Color` property | Public property, deserialized from JSON. |
| 26 | `TitleParameters.FontFamily` | `FontFamily` property | Public property, deserialized from JSON. |
| 27 | `TitleParameters.FontStyle` | `FontStyle` property (enum) | Public property, deserialized from JSON. |
| 28 | `TitleParameters(FontFamily, FontStyle, double, Color, bool, TitleVerticalAlignment)` | Constructor | Direct SD types in parameters. |

---

## Dependency Graph

```
Plugin Code
    |
    +-- SDConnection.SetImageAsync(Image) ........... [ADAPTED]
    +-- Tools.ImageToBase64(Image) .................. [ADAPTED]
    +-- Tools.Base64StringToImage(string) ........... [ADAPTED]
    +-- Tools.GenerateKeyImage(DeviceType, out Graphics) ... [NEEDS ADAPTER - returns Bitmap+Graphics]
    |       |
    |       +-- Plugin draws on Graphics, then calls SetImageAsync(Image)
    |
    +-- GraphicsTools.ResizeImage(Image) ............ [NEEDS ADAPTER]
    +-- GraphicsTools.CreateOpacityImage(Image) ..... [NEEDS ADAPTER]
    +-- GraphicsTools.DrawMultiLinedText(...) ........ [NEEDS ADAPTER - Font/Color params]
    +-- ExtensionMethods on Graphics ................ [HIGH-RISK - direct SD types]
    +-- TitleParameters (Color/FontFamily/FontStyle)  [HIGH-RISK - data class]
```

---

## Recommended Adapter Priority

1. **`Tools.GenerateKeyImage` / `GenerateGenericKeyImage`** -- Most commonly used plugin entry point for drawing. Adapting this unblocks the majority of plugin workflows.
2. **`GraphicsTools.ResizeImage` / `ExtractRectangle` / `CreateOpacityImage`** -- Image manipulation utilities, frequently used.
3. **`Image.ToByteArray`** -- Simple encoding extension, easy to route through codec.
4. **`GraphicsTools.DrawMultiLinedText`** -- Text rendering, depends on Font/Color.
5. **`TitleParameters`** -- Core data class. Needs careful deprecation strategy since plugins read these properties directly.
6. **Extension methods on `Graphics`** -- Lowest priority; these are advanced drawing helpers that tightly couple to `System.Drawing.Graphics`.
