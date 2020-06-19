using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Win32;
using Newtonsoft.Json;
// ReSharper disable EmptyGeneralCatchClause

namespace Launcher.Managers
{
    public static class FivemManager
    {
        public static void KillFivem()
        {
            var fivemProcess = Process.GetProcessesByName("fivem");
            foreach (var process in fivemProcess)
            {
                process.KillGorm();
            }
        }

        public static string GetFivemFolder()
        {
            var fivemShell = Registry.ClassesRoot.OpenSubKey("FiveM.ProtocolHandler\\shell\\open\\command");
            if (fivemShell == null) { return GetStaticFivemFolder(); }

            var cmd = fivemShell.GetValue(string.Empty)?.ToString();
            if (string.IsNullOrEmpty(cmd)) { return GetStaticFivemFolder(); }

            var fivemFolder = cmd.Contains(" ") ? cmd.Split(' ')[0].Replace("\"", string.Empty) : cmd.Replace("\"", string.Empty);
            if (!string.IsNullOrEmpty(fivemFolder) && fivemFolder.Length > 10)
            {
                var excludeExe = fivemFolder.Substring(0, fivemFolder.Length - 10);

                if (Directory.Exists($"{excludeExe}\\citizen\\common\\data\\ui\\"))
                {
                    fivemFolder = $"{excludeExe}\\citizen\\common\\data\\ui\\";
                }
                else if (Directory.Exists($"{excludeExe}\\FiveM.app\\citizen\\common\\data\\ui\\"))
                {
                    fivemFolder = $"{excludeExe}\\FiveM.app\\citizen\\common\\data\\ui\\";
                }
                else
                {
                    fivemFolder = string.Empty;
                }
            }

            return string.IsNullOrEmpty(fivemFolder) ? GetStaticFivemFolder() : fivemFolder;
        }

        private static string GetStaticFivemFolder()
        {
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var fivemFolder = $"{localAppData}\\FiveM\\FiveM.app\\citizen\\common\\data\\ui\\";
            return fivemFolder;
        }

        public static int GetModuleCount()
        {
            try
            {
                var fivemProcess = Process.GetProcessesByName("fivem");
                return fivemProcess.Select(process => process.Modules.Count).FirstOrDefault();
            }
            catch
            {
                return 0;
            }
        }
    }

    public partial class FivemApi
    {
        [JsonProperty("EndPoint")]
        public string EndPoint { get; set; }

        [JsonProperty("Data")]
        public Data Data { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("clients")]
        public long Clients { get; set; }

        [JsonProperty("gametype")]
        public string Gametype { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("mapname")]
        public string Mapname { get; set; }

        [JsonProperty("sv_maxclients")]
        public long DataSvMaxclients { get; set; }

        [JsonProperty("enhancedHostSupport")]
        public bool EnhancedHostSupport { get; set; }

        [JsonProperty("resources")]
        public string[] Resources { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("vars")]
        public Vars Vars { get; set; }

        [JsonProperty("players")]
        public Player[] Players { get; set; }

        [JsonProperty("connectEndPoints")]
        public Uri[] ConnectEndPoints { get; set; }

        [JsonProperty("upvotePower")]
        public long UpvotePower { get; set; }

        [JsonProperty("svMaxclients")]
        public long SvMaxclients { get; set; }

        [JsonProperty("lastSeen")]
        public DateTimeOffset LastSeen { get; set; }

        [JsonProperty("iconVersion")]
        public long IconVersion { get; set; }
    }

    public partial class Player
    {
        [JsonProperty("endpoint")]
        public string Endpoint { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("identifiers")]
        public string[] Identifiers { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("ping")]
        public long Ping { get; set; }
    }

    public partial class Vars
    {
        [JsonProperty("Developer")]
        public string Developer { get; set; }

        [JsonProperty("Dil")]
        public string Dil { get; set; }

        [JsonProperty("Discord")]
        public string Discord { get; set; }

        [JsonProperty("EssentialModeUUID")]
        public Guid EssentialModeUuid { get; set; }

        [JsonProperty("EssentialModeVersion")]
        public string EssentialModeVersion { get; set; }

        [JsonProperty("Hosting")]
        public string Hosting { get; set; }

        [JsonProperty("Kurucu")]
        public string Kurucu { get; set; }

        [JsonProperty("banner_connecting")]
        public string BannerConnecting { get; set; }

        [JsonProperty("banner_detail")]
        public string BannerDetail { get; set; }

        [JsonProperty("gamename")]
        public string Gamename { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("onesync_enabled")]
        public bool OnesyncEnabled { get; set; }

        [JsonProperty("sv_enhancedHostSupport")]
        public bool SvEnhancedHostSupport { get; set; }

        [JsonProperty("sv_lan")]
        public bool SvLan { get; set; }

        [JsonProperty("sv_licenseKeyToken")]
        public string SvLicenseKeyToken { get; set; }

        [JsonProperty("sv_maxClients")]
        public long SvMaxClients { get; set; }

        [JsonProperty("sv_scriptHookAllowed")]
        public bool SvScriptHookAllowed { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("vMenuUUID")]
        public string VMenuUuid { get; set; }

        [JsonProperty("vMenuVersion")]
        public string VMenuVersion { get; set; }
    }
}
