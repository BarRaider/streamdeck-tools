using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools
{

    /// <summary>
    /// FilenamePropertyAttribute - Used to indicate the current property holds a file name. 
    /// This will allow StreamDeck Tools to strip the mandatory "C:\fakepath\" added by the SDK
    /// </summary>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FilenamePropertyAttribute : Attribute
    {
    }
}
