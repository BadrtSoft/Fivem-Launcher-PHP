using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Win32;
using Newtonsoft.Json;

namespace Launcher.Managers
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

        public static string ConvertSteamID64(string steamID3)
        {
            var int64 = Convert.ToInt64(steamID3);
            return (int64 + 76561197960265728L).ToString();
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

        public static async Task<SteamPlayer> GetSteamProfile(string steamProxyURL, string steamId64)
        {
            if (string.IsNullOrEmpty(steamProxyURL) || string.IsNullOrEmpty(steamId64))
            {
                return null;
            }

            try
            {
                using (var webClient = new WebClient())
                {
                    var response = await webClient.DownloadStringTaskAsync(new Uri($"{steamProxyURL}?id={steamId64}"));
                    var obj = JsonConvert.DeserializeObject<SteamApi>(response);
                    return obj.Response?.SteamPlayers[0];
                }
            }
            catch
            {
                return null;
            }
        }

        public static string ConvertSteamIDHex(string steamID64)
        {
            return $"steam:{Convert.ToString(long.Parse(steamID64), 16)}";
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
        public SteamPlayer[] SteamPlayers { get; set; }
    }

    public partial class SteamPlayer
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