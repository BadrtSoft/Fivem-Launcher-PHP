using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Launcher
{
    public static class SteamManager
    {
        public static string GetSteamID3()
        {
            try
            {
                return Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("Valve")?.OpenSubKey("Steam")?.OpenSubKey("ActiveProcess")?.GetValue("ActiveUser")?.ToString() ?? "0";
            }
            catch
            {
                return "0";
                // ignored
            }
        }

        public static bool IsRunning()
        {
            return Process.GetProcessesByName("steam").Any();
        }
    }

    public partial class SteamApi
    {
        [JsonProperty("response")]
        public Response Response { get; set; }
    }

    public partial class Response
    {
        [JsonProperty("players")]
        public Player[] Players { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("steamid")]
        public string Steamid { get; set; }

        [JsonProperty("communityvisibilitystate")]
        public long Communityvisibilitystate { get; set; }

        [JsonProperty("profilestate")]
        public long Profilestate { get; set; }

        [JsonProperty("personaname")]
        public string Personaname { get; set; }

        [JsonProperty("profileurl")]
        public Uri Profileurl { get; set; }

        [JsonProperty("avatar")]
        public Uri Avatar { get; set; }

        [JsonProperty("avatarmedium")]
        public Uri Avatarmedium { get; set; }

        [JsonProperty("avatarfull")]
        public Uri Avatarfull { get; set; }

        [JsonProperty("avatarhash")]
        public string Avatarhash { get; set; }

        [JsonProperty("lastlogoff")]
        public long Lastlogoff { get; set; }

        [JsonProperty("personastate")]
        public long Personastate { get; set; }

        [JsonProperty("realname")]
        public string Realname { get; set; }

        [JsonProperty("primaryclanid")]
        public string Primaryclanid { get; set; }

        [JsonProperty("timecreated")]
        public long Timecreated { get; set; }

        [JsonProperty("personastateflags")]
        public long Personastateflags { get; set; }

        [JsonProperty("loccountrycode")]
        public string Loccountrycode { get; set; }

        [JsonProperty("locstatecode")]
        public long Locstatecode { get; set; }

        [JsonProperty("loccityid")]
        public long Loccityid { get; set; }
    }
}