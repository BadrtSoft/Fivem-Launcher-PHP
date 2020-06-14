using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Launcher.Managers
{
    public class UpdateManager
    {
        private string UpdateURL { get; }
        private string ExeLocation { get; }
        private string WorkingDirectory { get; }

        private UpdateObject UpdateResponse { get; set; }

        public UpdateManager(string updateUrl, string fullExePath)
        {
            UpdateURL = updateUrl;
            ExeLocation = fullExePath;
            WorkingDirectory = Path.GetDirectoryName(ExeLocation);
        }

        public async Task<UpdateObject> CheckUpdate()
        {
            UpdateResponse = await LauncherAPIManager.GetVariables(UpdateURL);
            return UpdateResponse;
        }

        public async Task<bool> DownloadUpdate()
        {
            var fileSegments = UpdateResponse.UpdateFile.Replace("\\", "/").Split('/');
            var updateFileName = fileSegments.Last();

            var fileExtension = Path.GetExtension(updateFileName).ToLower();
            if (!fileExtension.Equals(".zip"))
            {
                return false;
            }

            try
            {
                using (var webClient = new WebClient())
                {
                    await webClient.DownloadFileTaskAsync(UpdateResponse.UpdateFile, updateFileName);
                }
            }
            catch
            {
                return false;
            }

            var downloadedFiles = new List<string>();
            try
            {
                using (var zipArchive = ZipFile.OpenRead(updateFileName))
                {
                    foreach (var archiveEntry in zipArchive.Entries)
                    {
                        if (string.IsNullOrEmpty(archiveEntry.Name))
                        {
                            if (!Directory.Exists(archiveEntry.FullName))
                            {
                                Directory.CreateDirectory(archiveEntry.FullName);
                            }
                        }
                        else
                        {
                            archiveEntry.ExtractToFile($"{archiveEntry.FullName}.update", true);
                            downloadedFiles.Add($"{archiveEntry.FullName.Replace("/", "\\")}.update");
                        }
                    }
                }
            }
            catch
            {
                File.Delete(updateFileName);
                return false;
            }

            File.Delete(updateFileName);

            if (downloadedFiles.Count <= 0)
            {
                return false;
            }

            var fileName = Path.GetFileName(ExeLocation);
            var sb = new StringBuilder();
            sb.AppendLine("@ECHO OFF");
            sb.AppendLine("CHCP 65001");
            sb.AppendLine("TIMEOUT /t 1 /nobreak >NUL");
            sb.AppendLine($"TASKKILL /F /IM \"{fileName}\" >NUL");
            sb.AppendLine(":loop");
            sb.AppendLine($"TASKLIST /FI \"ImageName eq {fileName}\" /NH |find /i \"{fileName}\" >NUL && Goto :loop");

            foreach (var downloadedFile in downloadedFiles)
            {
                string updatedFileName;
                if (downloadedFile.Substring(downloadedFile.Length - 7, 7) == ".update")
                {
                    updatedFileName = downloadedFile.Substring(0, downloadedFile.Length - 7);
                }
                else
                {
                    updatedFileName = downloadedFile.Replace("_Update.", ".");
                }

                sb.AppendLine($"MOVE \"{WorkingDirectory}\\{downloadedFile}\" \"{WorkingDirectory}\\{updatedFileName}\"");
            }

            sb.AppendLine($"DEL \"%~f0\" & START \"\" /B /MAX \"{fileName}\" -updated");

            File.WriteAllText($"{WorkingDirectory}\\Update.bat", sb.ToString());

            return true;
        }

        public void InstallUpdate()
        {
            var startInfo = new ProcessStartInfo($"{WorkingDirectory}\\Update.bat")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory ?? string.Empty
            };

            Process.Start(startInfo);
        }
    }

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

        [JsonProperty("teamspeak3")]
        public string Teamspeak3 { get; set; }

        [JsonProperty("server_code")]
        public string ServerCode { get; set; }

        [JsonProperty("cheats")]
        public string[] Cheats { get; set; }
    }
}