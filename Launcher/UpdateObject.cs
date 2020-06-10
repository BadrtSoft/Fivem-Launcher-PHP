using Newtonsoft.Json;

namespace Launcher
{
    public class UpdateObject
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("update_file")]
        public string UpdateFile { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("discord")]
        public string Discord { get; set; }

        [JsonProperty("teamspeak")]
        public string Teamspeak3 { get; set; }

        [JsonProperty("server_code")]
        public string ServerCode { get; set; }

        [JsonProperty("cheats")]
        public string[] Cheats { get; set; }
    }
}