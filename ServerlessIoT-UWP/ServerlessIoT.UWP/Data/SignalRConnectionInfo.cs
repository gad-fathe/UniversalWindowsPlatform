using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerlessIoT.UWP.Data
{
    public class SignalRConnectionInfo
    {
        [JsonProperty("url")]
        public string Url { get; set; }
        [JsonProperty("accessToken")]
        public string AccessToken { get; set; }
    }
}
