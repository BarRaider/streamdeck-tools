# BarRaider's Stream Deck Tools

#### C# library that wraps all the communication with the Stream Deck App, allowing you to focus on actually writing the Plugin's logic.

[![NuGet](https://img.shields.io/nuget/v/streamdeck-tools.svg?style=flat)](https://www.nuget.org/packages/streamdeck-tools)

**Author's website and contact information:** [https://barraider.com](https://barraider.com)  

### Samples of plugins using this framework: [Samples][1]

### New tools included:
* [EasyPI](https://github.com/BarRaider/streamdeck-easypi) - Easily pass information from the PI (Property Inspector) to your plugin.  
* [Install.bat](https://github.com/BarRaider/streamdeck-tools/blob/master/utils/install.bat) - Script that quickly uninstalls and reinstalls your plugin on the streamdeck (view batch file for more details)  
* [StreamDeck-Tools Template](https://github.com/BarRaider/streamdeck-tools/raw/master/utils/StreamDeck-Tools%20Template.vsix) for Visual Studio - Automatically creates a project with all the files needed to compile a plugin

### Version 2.8 is out!
- Introduced `GraphicsUtils` class with a bunch of helper functions to manipulate the SD images
- Added new `Tools.FormatNumber()` function converts 54265 to 54.27k
- New ExtensionMethods for `Graphics` object: `DrawAndMeasureString` / `GetTextCenter`
- Updated dependency packages to latest versions
- Bug fix where SDConnection was not properly disposed.

### Version 2.7 is out!
- Fully wrapped all Stream Deck events (All part of the SDConneciton class). See ***"Subscribing to events"*** section below
- Added extension methods for multiple classes related to brushes/colors
- Added additional methods under the Tools class, including AddTextPathToGraphics which can be used to correctly position text on a key image based on the Text Settings in the Property Inspector see ***"Showing Title based on settings from Property Inspector"*** section below.
- Additional error checking
- Updated dependency packages to latest versions
- Sample plugin now included in this project on Github

### 2019-11-17
- Updated Install.bat (above) to newer version

### Version 2.6 is out!
- Added new MD5 functions in the `Tools` helper class
- Optimized SetImage to not resubmit an image that was just posted to the device. Can be overridden with new property in Connection.SetImage() function.

## Features
- Sample plugin now included in this project on Github
- Simplified working with filenames from the Stream Deck SDK. See ***"Working with files"*** section below
- Built-in integration with NLog. Use `Logger.LogMessage()` for logging. 
- Just call the `SDWrapper.Run()` and the library will take care of all the overhead
- Just have your plugin inherit PluginBase and implement the basic functionality. Use the PluginActionId to specify the UUID from the manifest file. (see samples on github page)
- Simplified receiving Global Settings updates through the new `ReceivedGlobalSettings` method
- Simplified receiving updates from the Property Inspector through the new `ReceivedSettings` method along with the new `Tools.AutoPopulateSettings()` method. See the ***"Auto-populating plugin settings"*** section below. 
- Introduced a new attribute called PluginActionId to indicate the Action's UUID (See below)
- Added support to switching plugin profiles.
- The DeviceId that the plugin is running on is now accessible from the `Connection` object
- Added new MD5 functions in the `Tools` helper class
- Optimized SetImage to not resubmit an image that was just posted to the device. Can be overridden with new property in Connection.SetImage() function.
- ExtensionMethods for Brush/Color/Graphics objects
- Helper functions in the `Tools` and `GraphicTools` classes

## How do I use this?
A list of plugins already using this library can be found [here][1]

This library wraps all the communication with the Stream Deck App, allowing you to focus on actually writing the Plugin's logic.
After creating a C# Console application, using this library requires two steps:

1. Create a class that inherits the PluginBase abstract class.  
Implement your logic, focusing on the methods provided in the base class.  
Follow the samples [here][1] for more details  
**New:** In version 2.x - use the `PluginActionId` attribute to indicate the action UUID associated with this class (must match the UUID set in the manifest file)

~~~~
[PluginActionId("plugin.uuid.from.manifest.file")]
public class MyPlugin : PluginBase
{
	// Create this constructor in your plugin and pass the objects to the PluginBase class
	public MyPlugin(SDConnection connection, InitialPayload payload) : base(connection, payload)
	{
		....
		// TODO: Use the payload.Settings to see the various settings set in the Property Inspector (in my samples, I create a private class that holds the settings)
		// Other relevant settings in the payload include the actual position of the plugin on the Stream Deck
		
		// Note: By passing the `connection` object back to the PluginBase (using the `base` in the constructor), you now have access to a property called `Connection` 
		// throughout your plugin.
	}
			....
			
	// TODO: Implement all the remaining abstract functions from PluginBase (or just leave them empty if you don't need them)
	
	// An example of how easy it is to populate settings in StreamDeck-Tools v2
	public override void ReceivedSettings(ReceivedSettingsPayload payload)
	{
		Tools.AutoPopulateSettings(settings, payload.Settings); // "settings" is a private class that holds the settings for your plugin's instance.
	}
}
~~~~

2. In your program.cs, just pass the args you received to the SDWrapper.Run() function, and you're done!  
**Note:** This process is much easier than the one used in 1.x and is based on using the `PluginActionId` attribute, as shown in Step 1 above.  
Example:
~~~~
class Program
{
	static void Main(string[] args)
	{
		SDWrapper.Run(args);
	}
}
~~~~

3. There is no step 3 - that's it! The abstract functions from PluginBase that are implemented in MyPlugin hold all the basics needed for a plugin to work. You can always listen to additional events using the `Connection` property.

## Auto-populating plugin settings
By following a very basic convention, the StreamDeck-Tools can handle populating all the settings between the PropertyInspector and your plugin. All the Stream-Deck Tools samples use this convention so you can see it in the samples too:
1. In your Plugin create a private class that will hold your plugin's settings. In the samples and in this example, we will call the private class `PluginSettings`
2. For each setting in your class, create a public property
3. For each one of the public properties add a JsonPropery attribute. The `PropertyName` field should be **identical** to the name of the setting's field in the PropertyInspector's payload.

```
private class PluginSettings
{
    [JsonProperty(PropertyName = "title")]
    public String Title { get; set; }
}
```
In the example above, we created a property named Title, and added a JsonProperty attribute with the `PropertyName` of `title`. This means in our Payload we should have a field with the name `title`

4. If you followed this for all your other properties, use the `Tools.AutoPopulateSettings()` method to Auto-populate all the properties inside your `ReceivedSettings` function:

```
public override void ReceivedSettings(ReceivedSettingsPayload payload) 
{
    Tools.AutoPopulateSettings(settings, payload.Settings);
}
```
Note: If you're using the filepicker, it's a little bit trickier:

## Working with files
The Stream Deck SDK automatically appends a "C:\fakepath\" to each file choosen through the SDK's filepicker. StreamDeck-Tools automatically can also auto-populate that field by adding an additional attribute named `FilenameProperty` to your property:
```
private class PluginSettings
{
    [FilenameProperty]
    [JsonProperty(PropertyName = "title")]
    public String Title { get; set; }
}
```
This will tell the `AutoPopulateSettings` method to strip the "C:\fakepath\" from the input.
But how do you make sure it shows correctly in the PropertyInspector too? Make sure you SAVE the settings back after StreamDeck-Tools fixes the filename:
```
public async override void ReceivedSettings(ReceivedSettingsPayload payload) 
{
    Tools.AutoPopulateSettings(settings, payload.Settings);
	// Return fixed filename back to the Property Inspector
	await Connection.SetSettingsAsync(JObject.FromObject(settings));
}
```

## Subscribing to events  
A full list of Stream Deck events are available [here](https://developer.elgato.com/documentation/stream-deck/sdk/events-received/). You can subscribe to them using the Connection object in the plugin. **IMPORTANT**: Remember to unsubscribe in the Dispose() function as shown below:

```
// Subscribe in Constructor

public MyPlugin(SDConnection connection, InitialPayload payload) : base(connection, payload)
{
	...
	...

	Connection.OnApplicationDidLaunch += Connection_OnApplicationDidLaunch;
	Connection.OnApplicationDidTerminate += Connection_OnApplicationDidTerminate;
	Connection.OnDeviceDidConnect += Connection_OnDeviceDidConnect;
	Connection.OnDeviceDidDisconnect += Connection_OnDeviceDidDisconnect;
	Connection.OnPropertyInspectorDidAppear += Connection_OnPropertyInspectorDidAppear;
	Connection.OnPropertyInspectorDidDisappear += Connection_OnPropertyInspectorDidDisappear;
	Connection.OnSendToPlugin += Connection_OnSendToPlugin;
	Connection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
}

...
	
// Unsubscribe in Dispose
public override void Dispose()
{
	Connection.OnApplicationDidLaunch -= Connection_OnApplicationDidLaunch;
	Connection.OnApplicationDidTerminate -= Connection_OnApplicationDidTerminate;
	Connection.OnDeviceDidConnect -= Connection_OnDeviceDidConnect;
	Connection.OnDeviceDidDisconnect -= Connection_OnDeviceDidDisconnect;
	Connection.OnPropertyInspectorDidAppear -= Connection_OnPropertyInspectorDidAppear;
	Connection.OnPropertyInspectorDidDisappear -= Connection_OnPropertyInspectorDidDisappear;
	Connection.OnSendToPlugin -= Connection_OnSendToPlugin;
	Connection.OnTitleParametersDidChange -= Connection_OnTitleParametersDidChange;
	Logger.Instance.LogMessage(TracingLevel.INFO, "Destructor called");
}

```

## Showing Title based on settings from Property Inspector  
The following is an example of how you can use the title settings built-in the property inspector to show it as an image on the key:

```
// Note this exists in SdTools.Wrappers namespace
private SdTools.Wrappers.TitleParameters titleParameters = null;
private string userTitle;

// Constructor
public MyPlugin(SDConnection connection, InitialPayload payload) : base(connection, payload)
{
	if (payload.Settings == null || payload.Settings.Count == 0)
	{
		this.settings = PluginSettings.CreateDefaultSettings();
	}
	else
	{
		this.settings = payload.Settings.ToObject<PluginSettings>();
	}
	// Get title information
	Connection.OnTitleParametersDidChange += Connection_OnTitleParametersDidChange;
}

private void Connection_OnTitleParametersDidChange(object sender, SdTools.Wrappers.SDEventReceivedEventArgs<SdTools.Events.TitleParametersDidChange> e)
{
	titleParameters = e.Event?.Payload?.TitleParameters;
	userTitle = e.Event?.Payload?.Title;
}

// Display on key with OnTick
public async override void OnTick()
{
	using (Bitmap img = Tools.GenerateGenericKeyImage(out Graphics graphics))
	{
		int height = img.Height;
        int width = img.Width;
		
		Tools.AddTextPathToGraphics(graphics, titleParameters, img.Height, img.Width, userTitle);
		await Connection.SetImageAsync(img);
        graphics.Dispose();
	}
}
```

[1]: https://github.com/BarRaider/streamdeck-tools/blob/master/samples.md