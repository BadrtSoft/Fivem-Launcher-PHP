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
                return Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam\\ActiveProcess")?.GetValue("ActiveUser")?.ToString() ?? "0";
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

        public static bool RunSteam()
        {
            try
            {
                var steamExe = Registry.CurrentUser.OpenSubKey("Software\\Valve\\Steam")?.GetValue("SteamExe")?.ToString();
                if (string.IsNullOrEmpty(steamExe))
                {
                    return false;
                }

                Process.Start(steamExe);
                return true;
            }
            catch
            {
                return false;
            }
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

        [JsonProperty("personaname")]
        public string Personaname { get; set; }

        [JsonProperty("profileurl")]
        public string Profileurl { get; set; }

        [JsonProperty("avatar")]
        public string Avatar { get; set; }

        [JsonProperty("avatarfull")]
        public string Avatarfull { get; set; }
    }
}