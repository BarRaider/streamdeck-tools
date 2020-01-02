using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BarRaider.SdTools.Payloads
{
    public class ApplicationPayload
    {
        [JsonProperty("application")]
        public string Application { get; private set; }

        public ApplicationPayload(string application)
        {
            Application = application;
        }
    }
}
