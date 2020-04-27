using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    /// <summary>
    /// ApplicationPayload
    /// </summary>
    public class ApplicationPayload
    {
        /// <summary>
        /// Application Name
        /// </summary>
        [JsonProperty("application")]
        public string Application { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="application"></param>
        public ApplicationPayload(string application)
        {
            Application = application;
        }
    }
}
