using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Wrappers
{
    /// <summary>
    /// Base (Generic) EventArgs used for events
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class SDEventReceivedEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Event Information
        /// </summary>
        public T Event { get; private set; }
        internal SDEventReceivedEventArgs(T evt)
        {
            this.Event = evt;
        }
    }
}
