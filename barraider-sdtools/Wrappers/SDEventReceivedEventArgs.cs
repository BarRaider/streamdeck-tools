using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Wrappers
{
    public class SDEventReceivedEventArgs<T> : EventArgs
    {
        public T Event { get; private set; }
        internal SDEventReceivedEventArgs(T evt)
        {
            this.Event = evt;
        }
    }
}
