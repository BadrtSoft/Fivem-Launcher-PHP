using Newtonsoft.Json;

namespace Launcher
{
    public partial class VarsObject
    {
        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("discord")]
        public string Discord { get; set; }

        [JsonProperty("teamspeak")]
        public string Teamspeak { get; set; }

        [JsonProperty("server_code")]
        public string ServerCode { get; set; }
    }
}