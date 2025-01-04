# BarRaider's Stream Deck Tools

#### C# library that wraps all the communication with the Stream Deck App, allowing you to focus on actually writing the Plugin's logic.

[![Build Status](https://github.com/BarRaider/streamdeck-tools/actions/workflows/dotnetcore.yml/badge.svg)](https://github.com/BarRaider/streamdeck-tools/actions/workflows/dotnetcore.yml)  [![NuGet](https://img.shields.io/nuget/v/streamdeck-tools.svg?style=flat)](https://www.nuget.org/packages/streamdeck-tools)

**Author's website and contact information:** [https://barraider.com](https://barraider.com)  

# Stream Deck+ Support  
Instead of `PluginBase`, Derive from either `KeypadBase` (if you don't support dials), `EncoderBase` (for only dials), `KeyAndEncoderBase` (for both keys and dials)

# Getting Started
Introducing our new [wiki](https://github.com/BarRaider/streamdeck-tools/wiki) packed with usage instructions, examples and more.

# Dev Discussions / Support
**Discord:** Discuss in #developers-chat in [Bar Raiders](http://discord.barraider.com)

## Downloadable Resources
* [StreamDeck-Tools Template](https://github.com/BarRaider/streamdeck-tools/raw/master/utils/StreamDeck-Tools%20Template.vsix) for Visual Studio (2019/2022) - Automatically creates a project with all the files needed to compile a plugin. This is the best way to start a new plugin!
* [Install.bat](https://github.com/BarRaider/streamdeck-tools/blob/master/utils/install.bat) - Script that quickly uninstalls and reinstalls your plugin on the streamdeck (edit the batch file for more details). Put the install.bat file in your BIN folder (same folder that has Debug/Release sub-folders)
* [EasyPI](https://github.com/BarRaider/streamdeck-easypi) - Additional library used to easily pass information from the PI (Property Inspector) to your plugin.
* [Profiles](https://barraider.com/profiles) Downloadable empty profiles for the XL (32-key), Classic (15-key), Mini (6-key) and Mobile devices at https://barraider.com/profiles

## Library Features
- Encapsulates all the communicating with the Stream Deck, getting a plugin working on the Stream Deck only requires implementing the PluginBase class.
- Sample plugin now included in this project on Github
- Built-in integration with NLog. Use `Logger.LogMessage()` for logging. 
- Auto-populate user settings which were modified by the Property Inspector
- Access the Global Settings from anywhere in your code
- Simplified working with filenames from the Stream Deck SDK.
- `PluginActionId` attribute let's you easily associate your code to a specific action defined in the manifest.json
- Large set of helper functions to simplify creating images and sending them to the Stream Deck.

# Change Log

### Version 6.3
- Support for new Stream Deck types

### Version 6.2
- Support for .NET 8.0

### Version 6.1
- Support for new `DialDown` and `DialUp` events. 
- Removed support for deprecated `DialPress` event

### Version 6.0
1. Merged streamdeck-client-csharp package into library to allow better logging of errors
2. Added support for SD+ SDK
3. Increased timeout of connection to Stream Deck due to the Stream Deck taking longer than before to reply on load
4. Added error catching to prevent 3rd party plugin exception to impact communication


### Version 3.2 is out!
- Created new `ISDConnection` interface which is now implemented by SDConnection and used by PluginAction.
- GlobalSettingsManager now has a short delay before calling GetGlobalSettings(), to reduce spamming the Stream Deck SDK.
- Updated dependencies to latest version

### Version 3.1 is out!
- Updated Logger class to include process name and thread id

### Version 3.0 is out!
- Updated file handling in `Tools.AutoPopulateSettings` and `Tools.FilenameFromPayload` methods
- Removed obsolete MD5 functions, use SHA512 functions instead
- `Tools.CenterText` function now has optional out `textFitsImage` value to verify the text does not exceed the image width
- New `Tools.FormatBytes` function converts bytes to human-readable value
- New `Graphics.GetFontSizeWhereTextFitsImage` function helps locate the best size for a text to fit an image on 1 line
- Updated dependency packages to latest versions
- Bug fix where FileNameProperty attribute

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

