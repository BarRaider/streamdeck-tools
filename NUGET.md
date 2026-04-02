# BarRaider's Stream Deck Tools

#### C# library that wraps all the communication with the Stream Deck App, allowing you to focus on actually writing the Plugin's logic.

[![NuGet](https://img.shields.io/nuget/v/streamdeck-tools.svg?style=flat)](https://www.nuget.org/packages/streamdeck-tools)

**Author's website and contact information:** [https://barraider.com](https://barraider.com)

## Quick Start

1. Derive from `KeypadBase` (keys), `EncoderBase` (dials), or `KeyAndEncoderBase` (both)
2. Decorate with `[PluginActionId("your.action.uuid")]`
3. Call `SDWrapper.Run(args)` in `Main()`

```csharp
[PluginActionId("com.example.myplugin")]
public class MyAction : KeypadBase
{
    public MyAction(ISDConnection connection, InitialPayload payload) : base(connection, payload) { }

    public async override void KeyPressed(KeyPayload payload)
    {
        using SKBitmap image = SkiaTools.GenerateGenericKeyImage(out SKCanvas canvas);
        canvas.Clear(SKColors.DarkBlue);
        using var font = SkiaTools.CreateFont("Arial", 18, SKFontStyle.Bold);
        using var paint = new SKPaint { Color = SKColors.White, IsAntialias = true };
        canvas.DrawTextLine("Hello!", font, paint, new SKPoint(30, 60));
        canvas.Dispose();
        await Connection.SetImageAsync(image);
    }

    public override void KeyReleased(KeyPayload payload) { }
    public override void OnTick() { }
    public override void Dispose() { }
    public override void ReceivedSettings(ReceivedSettingsPayload payload) { }
    public override void ReceivedGlobalSettings(ReceivedGlobalSettingsPayload payload) { }
}
```

## Features

- Encapsulates all Stream Deck communication -- just implement your action logic
- Cross-platform SkiaSharp-based graphics API (`SkiaTools`, `SkiaGraphicsTools`, `SkiaExtensionMethods`)
- Target frameworks: netstandard2.0, net48, net8.0, net10.0
- Built-in NLog integration via `Logger.LogMessage()`
- Auto-populate settings from the Property Inspector with `Tools.AutoPopulateSettings()`
- Global Settings support
- `PluginActionId` attribute for action UUID mapping
- Stream Deck+ support (keys, dials, and combined)

## Migrating from v6.x

See the full [Migration Guide](https://github.com/BarRaider/streamdeck-tools/blob/master/MIGRATION.md) for type mapping tables, code recipes, and step-by-step instructions.

## Resources

- [GitHub Repository](https://github.com/BarRaider/streamdeck-tools)
- [Wiki](https://github.com/BarRaider/streamdeck-tools/wiki)
- [Sample Plugins](https://github.com/BarRaider/streamdeck-tools/blob/master/samples.md)
- [EasyPI](https://github.com/BarRaider/streamdeck-easypi)
- [Discord](http://discord.barraider.com)
