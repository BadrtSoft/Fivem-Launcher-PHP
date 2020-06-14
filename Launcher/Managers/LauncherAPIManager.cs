using System;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Launcher.Managers
{
    public static class LauncherAPIManager
    {
        public static async Task<string> GetStatus(string serverCheckURL, string steamHex)
        {
            try
            {
                string durum;
                using (var webClient = new WebClient())
                {
                    durum = await webClient.DownloadStringTaskAsync(new Uri($"{serverCheckURL}?steamid={steamHex}"));
                }

                return durum;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<string> SetStatus(string serverUpdateURL, string steamHex, string status)
        {
            try
            {
                string durum;

                using (var webClient = new WebClient())
                {
                    var data = $"steamid={steamHex}&durum={status}";
                    durum = await webClient.UploadStringTaskAsync(new Uri($"{serverUpdateURL}?{data}"), data);
                }

                return durum;
            }
            catch
            {
                return null;
            }
        }

        public static async Task<UpdateObject> GetVariables(string launcherUpdateURL)
        {
            try
            {
                UpdateObject globalVariables;

                using (var webClient = new WebClient())
                {
                    var response = await webClient.DownloadStringTaskAsync(new Uri($"{launcherUpdateURL}"));
                    globalVariables = JsonConvert.DeserializeObject<UpdateObject>(response);
                }

                return globalVariables;
            }
            catch
            {
                return null;
            }
        }

    }
}