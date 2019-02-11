# BarRaider's Stream Deck Tools

[![NuGet](https://img.shields.io/nuget/v/streamdeck-tools.svg?style=flat)](https://www.nuget.org/packages/streamdeck-tools)

**Author's website and contact information:** [https://barraider.github.io](https://barraider.github.io)

## Features
- Just call the `SDWrapper.Run()` and the library will take care of all the overhead
- Built-in integration with NLog. Use `Logger.LogMessage()` for logging. 
- Just have your plugin inherit PluginBase and implement the basic functionality

## How do I use this?
This library wraps all the communication with the Stream Deck App, allowing you to focus on actually writing the Plugin's logic.
After creating a C# Console application, using this library requires two steps:

1. Create a class that inherits the PluginBase abstract class.
Implement your logic, focusing on the methods provided in the base class.
Follow the sample here for more details: https://github.com/BarRaider/barraider-sdtools

~~~~
public class MyPlugin : PluginBase
{
	// Create this constructor in your plugin and pass the objects to the PluginBase class
	public MyPlugin(SDConnection connection, JObject settings) : base(connection, settings)
	{
		...
	}
			....
}
~~~~

2. In your program.cs, Create a list of all the ActionIds (UUIDs) your plugin supports, and which class implements it.
The actionId correlates to the UUID field in the manifest.json file.  
You can have more than one action in the manifest file and therefore we except an array.
The second parameter is the type (not instance!) of the class that implements that action. This is the class from step 1 above.
Pass the args you received along with the supported ActionIds to the SDWrapper.Run() function, and you're done!
Example:
~~~~
class Program
{
	static void Main(string[] args)
	{
		List<PluginActionId> supportedActionIds = new List<PluginActionId>();
		supportedActionIds.Add(new PluginActionId("com.barraider.myUUID", typeof(MyPlugin)));

		SDWrapper.Run(args, supportedActionIds.ToArray());
	}
}
~~~~