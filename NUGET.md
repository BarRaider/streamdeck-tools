# BarRaider's Stream Deck Tools

#### C# library that wraps all the communication with the Stream Deck App, allowing you to focus on actually writing the Plugin's logic.

[![NuGet](https://img.shields.io/nuget/v/streamdeck-tools.svg?style=flat)](https://www.nuget.org/packages/streamdeck-tools)

**Author's website and contact information:** [https://barraider.github.io](https://barraider.github.io)  
** Samples of plugins using this framework: [Samples][1]

### Version 2.5 is out!
- Added support for StreamDeckMobile device type
- Added new `Tools.GenerateGenericKeyImage()` function that generates an image that fits all Stream Decks

### Version 2.4.2 is out!
- Added support for Plugin version inside the StreamDeckInfo class. 
- Current plugin version is now shown in the `pluginlog.log` log file
- Publishing through `GlobalSettingsManager` now raises the GlobalSettingsReceived event by default

### Version 2.3 is out!
- Added support for StreamDeck XL
1. `Connection.DeviceInfo()` now gives you information on the device the plugin is on (including the `StreamDeckDeviceType`)
2. `StreamDeckDeviceType` enum now recognizes the StreamDeck XL
3. KEY_DEFAULT_WIDTH and KEY_DEFAULT_HEIGHT from the `Tools` module now replaced with dedicated `GetKeyDefaultWidth()`/`GetKeyDefaultHeight()` methods based on the StreamDeckDeviceType. This is due to different resoultion on the XL
- New GlobalSettingsManager class gives you direct access to the Plugin's global settings from anywhere in your code

## Features
- Simplified working with filenames from the Stream Deck SDK. See ***"Working with files"*** section below
- Built-in integration with NLog. Use `Logger.LogMessage()` for logging. 
- Just call the `SDWrapper.Run()` and the library will take care of all the overhead
- Just have your plugin inherit PluginBase and implement the basic functionality. Use the PluginActionId to specify the UUID from the manifest file. (see samples on github page)

- Optimized for the Stream Deck 4.1 SDK
- Added support for GlobalSettings (introduced in the 4.1 SDK)
- Simplified receiving Global Settings updates through the new `ReceivedGlobalSettings` method
- Simplified receiving updates from the Property Inspector through the new `ReceivedSettings` method along with the new `Tools.AutoPopulateSettings()` method. See the ***"Auto-populating plugin settings"*** section below. 
- Introduced a new attribute called PluginActionId to indicate the Action's UUID (See below)
- Added support to switching plugin profiles.
- The DeviceId that the plugin is running on is now accessible from the `Connection` object

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

-------
### Additional step-by-step instructions, including how to upgrade from v1.0 are [here](https://github.com/BarRaider/streamdeck-tools/blob/master/README.md)
-------

[1]: https://github.com/BarRaider/streamdeck-tools/blob/master/samples.md