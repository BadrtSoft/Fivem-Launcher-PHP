using Newtonsoft.Json;

namespace Launcher
{
    public class UpdateObject
    {
        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("file")]
        public string File { get; set; }
    }
}