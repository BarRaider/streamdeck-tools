using BarRaider.SdTools;
using CommandLine;
using System;

namespace BarRaider.SdTools
{
    /// <summary>
    /// * Easy Configuration Instructions:
    ///* 1. Use NuGet to get the following packages: 
    ///*          CommandLineParser by gsscoder
    ///*          streamdeck-client-csharp by Shane DeSeranno
    ///*          Newtonsoft.Json by James Newton-King
    ///* 2. Create a class that implements the IPluginable interface (which is located in BarRaider.SDTools), this will be your main plugin
    ///* 3. Pass the type of the class to the main function
    /// </summary>
    public static class SDWrapper
    {
        // Handles all the communication with the plugin
        private static PluginContainer container;

        /************************************************************************
       * Initial configuration from TyrenDe's streamdeck-client-csharp example:
       * https://github.com/TyrenDe/streamdeck-client-csharp
       * and SaviorXTanren's MixItUp.StreamDeckPlugin:
       * https://github.com/SaviorXTanren/mixer-mixitup/
       *************************************************************************/

        // StreamDeck launches the plugin with these details
        // -port [number] -pluginUUID [GUID] -registerEvent [string?] -info [json]
        public static void Run(string[] args, PluginActionId[] supportedActionIds)
        {
            Logger.Instance.LogMessage(TracingLevel.INFO, "Plugin Loading");
            System.AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }

            // The command line args parser expects all args to use `--`, so, let's append
            for (int count = 0; count < args.Length; count++)
            {
                if (args[count].StartsWith("-") && !args[count].StartsWith("--"))
                {
                    args[count] = $"-{args[count]}";
                }
            }

            Parser parser = new Parser((with) =>
            {
                with.EnableDashDash = true;
                with.CaseInsensitiveEnumValues = true;
                with.CaseSensitive = false;
                with.IgnoreUnknownArguments = true;
                with.HelpWriter = Console.Error;
            });

            ParserResult<StreamDeckOptions> options = parser.ParseArguments<StreamDeckOptions>(args);
            options.WithParsed<StreamDeckOptions>(o => RunPlugin(o, supportedActionIds));
        }

        private static void RunPlugin(StreamDeckOptions options, PluginActionId[] supportedActionIds)
        {
            container = new PluginContainer(supportedActionIds);
            container.Run(options);
        }

        private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Instance.LogMessage(TracingLevel.FATAL, $"Unhandled Exception: {e.ExceptionObject}");
        }
    }
}