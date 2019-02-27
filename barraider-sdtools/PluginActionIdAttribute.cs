using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{
    /// <summary>
    /// PluginActionId attribute
    /// Used to indicate the UUID in the manifest file that matches to this class
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class PluginActionIdAttribute : Attribute
    {

        /// <summary>
        /// UUID of the action
        /// </summary>
        public string ActionId { get; private set; }

        /// <summary>
        /// Constructor - This attribute is used to indicate the UUID in the manifest file that matches to this class
        /// </summary>
        /// <param name="ActionId"></param>
        public PluginActionIdAttribute(string ActionId)
        {
            this.ActionId = ActionId;
        }
    }
}
