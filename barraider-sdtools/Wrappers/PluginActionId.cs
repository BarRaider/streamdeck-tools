using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
    /// <summary>
    /// This class associates a plugin UUID (which is indicated in the Manifest file), with the type of the implementation class.
    /// The implementation class must be derived from the PluginBase class for this to work properly.
    /// If the type passed does not derrive from PluginBase, a NotSupportedException will be thrown
    /// </summary>
    public class PluginActionId
    {
        private Type pluginBaseType;

        /// <summary>
        /// Action UUID as indicated in the manifest file
        /// </summary>
        public string ActionId { get; private set; }

        /// <summary>
        /// Type of class that implemented this action. Must inherit PluginBase
        /// </summary>
        public Type PluginBaseType
        {
            get
            {
                return pluginBaseType;
            }
            private set
            {
                if (value == null || (!typeof(IKeypadPlugin).IsAssignableFrom(value) && !typeof(IEncoderPlugin).IsAssignableFrom(value)))
                {
                    throw new NotSupportedException("Class type set to PluginBaseType does not inherit IKeypadPlugin or IEncoderPlugin");
                }
                pluginBaseType = value;
            }
        }

        /// <summary>
        /// PluginActionId constructor
        /// </summary>
        /// <param name="actionId">actionId is the UUID from the manifest file</param>
        /// <param name="pluginBaseType">Type of class that implemented this action. Must inherit PluginBase</param>
        public PluginActionId(string actionId, Type pluginBaseType)
        {
            ActionId = actionId;
            PluginBaseType = pluginBaseType;
        }
    }
}
