using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Launcher
{
    public class UpdateManager
    {
        private string UpdateURL { get; set; }
        private UpdateObject UpdateResponse { get; set; }
        private string WorkingDirectory { get; set; }
        private string Location { get; set; }

        public UpdateManager(string updateUrl)
        {
            UpdateURL = updateUrl;
        }

        public async Task<UpdateObject> CheckUpdate()
        {
            using (var webClient = new WebClient())
            {
                var response = await webClient.DownloadStringTaskAsync(UpdateURL);
                var updateObject = JsonConvert.DeserializeObject<UpdateObject>(response);

                UpdateResponse = updateObject;
                return UpdateResponse;
            }
        }

        public async Task<bool> DownloadUpdate(string fullExePath)
        {
            var fileList = new[] { UpdateResponse.UpdateFile };
            var downloadedFiles = new List<string>();

            Location = fullExePath;
            WorkingDirectory = Path.GetDirectoryName(Location);
            var fileName = Path.GetFileName(Location);

            foreach (var fileUrl in fileList)
            {
                var fileSegments = fileUrl.Split('/');
                var updateFileName = fileSegments[fileSegments.Length - 1];

                using (var client = new WebClient())
                {
                    try
                    {
                        client.DownloadFile(fileUrl, updateFileName);
                        var ext = Path.GetExtension(updateFileName);
                        if (ext == ".zip")
                        {
                            var zipArchive = ZipFile.OpenRead(updateFileName);
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
                            zipArchive.Dispose();
                            File.Delete(updateFileName);
                        }
                        else
                        {
                            downloadedFiles.Add(updateFileName);
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            if (downloadedFiles.Count <= 0)
            {
                return false;
            }

            var sb = new StringBuilder();

            sb.AppendLine("@ECHO OFF");
            sb.AppendLine("CHCP 65001");
            sb.AppendLine("TIMEOUT /t 1 /nobreak >NUL");
            sb.AppendLine($"TASKKILL /F /IM \"{fileName}\" >NUL");
            sb.AppendLine(":loop");
            sb.AppendLine($"TASKLIST /FI \"ImageName eq {fileName}\" /NH |find /i \"{fileName}\" >NUL && Goto :loop");

            foreach (var downloadedFile in downloadedFiles)
            {
                sb.AppendLine(downloadedFile.Substring(downloadedFile.Length - 7, 7) == ".update"
                    ? $"MOVE \"{WorkingDirectory}\\{downloadedFile}\" \"{WorkingDirectory}\\{downloadedFile.Substring(0, downloadedFile.Length - 7)}\""
                    : $"MOVE \"{WorkingDirectory}\\{downloadedFile}\" \"{WorkingDirectory}\\{downloadedFile.Replace("_Update.", ".")}\"");
            }

            sb.AppendLine($"DEL \"%~f0\" & START \"\" /B /MAX \"{fileName}\" -updated");

            File.WriteAllText($"{WorkingDirectory}\\Update.bat", sb.ToString());

            return true;
        }

        public async Task<bool> InstallUpdate(string fullExePath = "")
        {
            if (string.IsNullOrEmpty(WorkingDirectory))
            {
                if (string.IsNullOrEmpty(fullExePath))
                {
                    return false;
                }

                Location = fullExePath;
                WorkingDirectory = Path.GetDirectoryName(Location);
            }

            var startInfo = new ProcessStartInfo($"{WorkingDirectory}\\Update.bat")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = WorkingDirectory ?? string.Empty
            };

            Process.Start(startInfo);

            return true;
        }
    }
}