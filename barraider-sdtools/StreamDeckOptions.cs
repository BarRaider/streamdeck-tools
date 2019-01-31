using CommandLine;
using Newtonsoft.Json.Linq;

namespace BarRaider.SdTools
{
    public class StreamDeckOptions
    {
        [Option("port", Required = true, HelpText = "The websocket port to connect to", SetName = "port")]
        public int Port { get; set; }

        [Option("pluginUUID", Required = true, HelpText = "The UUID of the plugin")]
        public string PluginUUID { get; set; }

        [Option("registerEvent", Required = true, HelpText = "The event triggered when the plugin is registered?")]
        public string RegisterEvent { get; set; }

        [Option("info", Required = true, HelpText = "Extra JSON launch data")]
        public string Info { get; set; }
    }
}
