using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarRaider.SdTools
{
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
                if (value == null || !value.IsSubclassOf(typeof(PluginBase)))
                {
                    throw new NotSupportedException("Class type set to PluginBaseType does not inherit PluginBase");
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
