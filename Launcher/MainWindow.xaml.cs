using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Newtonsoft.Json;
using Microsoft.Win32;
using SteamKit2;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int _kontrolEdilen;
        private int _hile;
        private string _steamHex;
        private VarsObject _varsObject;
        private readonly bool _isLocal;
        private bool _steamAcik;

        public MainWindow()
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1 && args[1].Equals("-local"))
            {
                _isLocal = true;
            }

            InitializeComponent();
        }

        private void SteamKapali()
        {
            MessageBox.Show("Oyuna bağlanabilmek için Steam açmalısın!", "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var steamId3 = Registry.CurrentUser.OpenSubKey("Software")?.OpenSubKey("Valve")?.OpenSubKey("Steam")?.OpenSubKey("ActiveProcess")?.GetValue("ActiveUser")?.ToString() ?? string.Empty;
            if (steamId3 == "0")
            {
                SteamKapali();
            }
            else
            {
                var steam = new SteamID(Convert.ToUInt32(steamId3), EUniverse.Public, EAccountType.Individual);
                var steamId64 = steam.ConvertToUInt64().ToString();

                var player = GetSteamPlayer(steamId64);
                if (player == null || string.IsNullOrEmpty(player.Personaname))
                {
                    SteamGecersiz();
                }
                else
                {
                    _steamAcik = true;

                    try
                    {
                        var appFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                                        "\\FiveM\\FiveM.app\\citizen\\common\\data\\ui\\";
                        File.WriteAllBytes(appFolder + "mapzoomdata.meta", Properties.Resources.mapzoomdata);
                        File.WriteAllBytes(appFolder + "pausemenu.xml", Properties.Resources.pausemenu_xml);
                    }
                    catch
                    {
                        // ignored
                    }

                    using (var webClient = new WebClient())
                    {
                        var response = webClient.DownloadString("https://yalc.in/fivem_launcher/vars.php");
                        _varsObject = JsonConvert.DeserializeObject<VarsObject>(response);

                        if (string.IsNullOrEmpty(_varsObject?.ServerCode)) LblOnline.Visibility = Visibility.Collapsed;
                    }

                    if (!_isLocal)
                    {
                        #region Updater

                        // UPDATER
                        var args = Environment.GetCommandLineArgs();
                        if (args.Any(a => a.Equals("-updated")))
                        {
                            MessageBox.Show("Launcher güncellendi!", "GormYa Launcher", MessageBoxButton.OK,
                                MessageBoxImage.Information);
                        }
                        else
                        {
                            Task.Run(async () =>
                            {
                                var updater = new UpdateManager("https://yalc.in/fivem_launcher/update.php");
                                var updateResponse =
                                    await updater.CheckUpdate(Assembly.GetExecutingAssembly().GetName().Version.ToString());

                                if (updateResponse != null)
                                {
                                    var isDownloaded =
                                        await updater.DownloadUpdate(Assembly.GetExecutingAssembly().Location);
                                    if (isDownloaded)
                                    {
                                        Dispatcher.Invoke(delegate { Visibility = Visibility.Hidden; });

                                        MessageBox.Show(
                                            "Launcher güncellenecektir. Kapatılıp açılırken lütfen bekleyiniz...",
                                            "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Information);
                                        await updater.InstallUpdate();
                                    }
                                }
                            });
                        }

                        #endregion
                    }

                    CheatProgramlariniKapat(null, null);

                    var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(30) };
                    timer.Tick += CheatProgramlariniKapat;
                    timer.Start();
                }
            }
        }

        private void SteamGecersiz()
        {
            MessageBox.Show("Steam bilgilerinizi okuyamadık! Steam'in açık olduğundan emin olun.", "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }

        private void CheatProgramlariniKapat(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    if (string.IsNullOrEmpty(_varsObject.ServerCode)) return;

                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadStringTaskAsync(new Uri(
                                "https://servers-frontend.fivem.net/api/servers/single/" + _varsObject.ServerCode))
                            .ContinueWith(task =>
                            {
                                var obj = JsonConvert.DeserializeObject<FivemApi>(task.Result);
                                Dispatcher.Invoke(delegate { LblOnline.Content = $"Online: {obj.Data.Clients}"; });
                            });
                    }
                }
                catch
                {
                    // ignored
                }
            });

            Task.Run(() =>
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadStringTaskAsync(new Uri("https://yalc.in/fivem_launcher/cheat.php")).ContinueWith(task =>
                    {
                        var cheatList = JsonConvert.DeserializeObject<List<string>>(task.Result);

                        var processes = Process.GetProcesses();
                        foreach (var process in processes)
                        {
                            if (cheatList.Any(s => s.Equals(process.ProcessName, StringComparison.OrdinalIgnoreCase)))
                            {
                                process.Kill();
                                _hile++;
                                // TODO: Hile report olacak
                            }
                            else
                            {
                                _kontrolEdilen++;
                            }
                        }
                    });
                }
            });
        }

        private Player GetSteamPlayer(string steamid)
        {
            try
            {
                string response;
                using (var webClient = new WebClient())
                {
                    response = webClient.DownloadString($"https://yalc.in/fivem_launcher/steamProxy.php?id={steamid}");
                }

                var obj = JsonConvert.DeserializeObject<SteamApi>(response);
                _steamHex = "steam:" + Convert.ToString(long.Parse(steamid), 16);
                return obj.Response.Players[0];
            }
            catch
            {
                return null;
            }
        }

        private void btnDiscord_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(_varsObject.Discord);
        }

        private void btnTeamspeak_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($"ts3server://{_varsObject.Teamspeak}");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void btnLaunch_Click(object sender, RoutedEventArgs e)
        {
            if (_kontrolEdilen <= 0 || _hile > 0)
            {
                MessageBox.Show("Sunucu kaydınız yapılamadı. Daha sonra tekrar deneyin."); // hile yakalandı veya kontrol edilemedi
                return;
            }

            try
            {
                using (var webClient = new WebClient())
                {
                    var r = webClient.UploadString($"https://yalc.in/fivem_launcher/guncelle.php?steamid={_steamHex}&durum=1", "POST", $"steamid={_steamHex}&durum=1");
                    if (r == "1")
                    {
                        var processName = $"fivem://connect/{(_isLocal ? "localhost:30120" : _varsObject.Server)}";
                        Process.Start(processName);
                    }
                    else
                    {
                        MessageBox.Show("Sunucu kaydınız yapılamadı. Daha sonra tekrar deneyin.");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Sunucu kaydınız yapılamadı. Daha sonra tekrar deneyin.");
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            MouseLeftButtonDown += delegate { DragMove(); };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                var fivemProcess = Process.GetProcessesByName("Fivem");
                foreach (var process in fivemProcess)
                {
                    process.Kill();
                }
            }
            catch
            {
                // ignored
            }

            if (!_steamAcik) return;

            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.UploadString($"https://yalc.in/fivem_launcher/guncelle.php?steamid={_steamHex}&durum=0", "POST", $"steamid={_steamHex}&durum=0");
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
