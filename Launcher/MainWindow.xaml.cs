using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
using Newtonsoft.Json;
using SteamKit2;

namespace Launcher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string LauncherUpdateURL = "https://yalc.in/fivem_launcher/update.php";
        private const string ServerUpdateURL = "https://yalc.in/fivem_launcher/guncelle.php";
        private const string ServerCheckURL = "https://yalc.in/fivem_launcher/kontrol.php";
        private const string SteamProxyURL = "https://yalc.in/fivem_launcher/steamProxy.php";

        private int _kontrolEdilen;
        private int _hile;
        private string _steamHex;
        private UpdateObject _updateObject;
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

        private void ShowError(string message, bool halt = true)
        {
            Visibility = Visibility.Hidden;
            MessageBox.Show(message, "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Error);

            if (halt)
            {
                Application.Current.Shutdown();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            if (args.Any(a => a.Equals("-updated")))
            {
                MessageBox.Show("Launcher güncellendi!", "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Information);
                UpdateKontrolEdildi();
            }
            else
            {
                Task.Run(async () =>
                {
                    var currentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    var updater = new UpdateManager(LauncherUpdateURL);
                    _updateObject = await updater.CheckUpdate();

                    if (_updateObject.Version != currentVersion)
                    {
                        var isDownloaded = await updater.DownloadUpdate(Assembly.GetExecutingAssembly().Location);
                        if (isDownloaded)
                        {
                            Dispatcher.Invoke(delegate { Visibility = Visibility.Hidden; });

                            MessageBox.Show("Launcher güncellenecektir. Kapatılıp açılırken lütfen bekleyiniz...", "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Information);
                            await updater.InstallUpdate();
                        }
                        else
                        {
                            Dispatcher.Invoke(UpdateKontrolEdildi);
                        }
                    }
                    else
                    {
                        Dispatcher.Invoke(UpdateKontrolEdildi);
                    }
                });
            }

            // renkli haritayı fivem klasörüne kopyala
            try
            {
                var fivemFolder = string.Empty;
                var fivemShell = Registry.ClassesRoot.OpenSubKey("FiveM.ProtocolHandler\\shell\\open\\command");
                if (fivemShell != null)
                {
                    var cmd = fivemShell.GetValue("")?.ToString();
                    if (!string.IsNullOrEmpty(cmd))
                    {
                        if (cmd.Contains(" "))
                        {
                            fivemFolder = cmd.Split(' ')[0].Replace("\"", string.Empty);
                        }
                        else
                        {
                            fivemFolder = cmd.Replace("\"", string.Empty);
                        }

                        if (!string.IsNullOrEmpty(fivemFolder))
                        {
                            fivemFolder = $"{fivemFolder.Substring(0, fivemFolder.Length - 10)}\\FiveM.app\\citizen\\common\\data\\ui\\";
                        }
                    }
                }

                if (string.IsNullOrEmpty(fivemFolder))
                {
                    var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    fivemFolder = $"{localAppData}\\FiveM\\FiveM.app\\citizen\\common\\data\\ui\\";
                }

                File.WriteAllBytes($"{fivemFolder}mapzoomdata.meta", Properties.Resources.mapzoomdata);
                File.WriteAllBytes($"{fivemFolder}pausemenu.xml", Properties.Resources.pausemenu_xml);
            }
            catch (Exception err)
            {
                MessageBox.Show("Harita dosyaları kopyalanamadı.", "GormYa Launcher", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void UpdateKontrolEdildi()
        {
            bool steamYeniAcildi = false;
            // Steam çalışıyor mu kontrol et
            _steamAcik = SteamManager.IsRunning();
            if (!_steamAcik)
            {
                if (MessageBox.Show($"Steam açık değil ve bu şekilde sunucuya bağlanamazsın.{Environment.NewLine}Açmamı ister misin?", "GormYa Launcher", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    if (SteamManager.RunSteam())
                    {
                        steamYeniAcildi = true;
                    }
                    else
                    {
                        ShowError("Steam'i açamadım. Sen benim yerime açıp, tekrar beni çalıştırabilirsin :)");
                        return;
                    }
                }
                else
                {
                    ShowError("Bir sonraki sefere görüşmek üzere :)");
                    return;
                }
            }

            var steamIdOkumaDenemesi = 0;
            steamIdOku:
            // SteamID3 okunabiliyor mu kontrol et
            var steamId3 = SteamManager.GetSteamID3();
            if (string.IsNullOrEmpty(steamId3) || steamId3.Equals("0"))
            {
                if (steamYeniAcildi)
                {
                    if (steamIdOkumaDenemesi <= 90)
                    {
                        steamIdOkumaDenemesi++;
                        Thread.Sleep(1000);
                        goto steamIdOku;
                    }

                    ShowError("Oyuna bağlanabilmek için Steam girişi yapmış olmalısın!");
                    return;
                }

                ShowError("Oyuna bağlanabilmek için Steam girişi yapmış olmalısın!");
                return;
            }

            var steam = new SteamID(Convert.ToUInt32(steamId3), EUniverse.Public, EAccountType.Individual);
            var steamId64 = steam.ConvertToUInt64().ToString();
            if (string.IsNullOrEmpty(steamId64) || steamId64.Equals("0"))
            {
                ShowError("Oyuna bağlanabilmek için Steam girişi yapmış olmalısın!");
                return;
            }

            // Steam api'den kullanıcı bilgilerini çek ve kontrol et
            var player = GetSteamPlayer(steamId64);
            if (player == null || string.IsNullOrEmpty(player.Personaname))
            {
                ShowError("Steam bilgilerinizi okuyamadık! Açık olduğundan ve giriş yaptığınızdan emin olun.");
                return;
            }

            _steamAcik = true;

            // Discord boş değilse butonunu göster
            if (!string.IsNullOrEmpty(_updateObject.Discord)) BtnDiscord.Visibility = Visibility.Visible;

            // TS3 boş değilse butonunu göster
            if (!string.IsNullOrEmpty(_updateObject.Teamspeak3)) BtnTeamspeak.Visibility = Visibility.Visible;

            // Server code boş değilse online sayısını göster
            if (!string.IsNullOrEmpty(_updateObject?.ServerCode)) LblOnline.Visibility = Visibility.Visible;

            // Server boş değilse butonunu göster
            if (!string.IsNullOrEmpty(_updateObject?.Server)) BtnLaunch.Visibility = Visibility.Visible;

            // Hile koruması
            CheatProgramlariniKapat(null, null);

            // 25 saniyede bir hile korumasını çalıştır
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(25) };
            timer.Tick += CheatProgramlariniKapat;
            timer.Start();
        }

        private void CheatProgramlariniKapat(object sender, EventArgs e)
        {
            // Durum guncelle (tarih guncellesin ki, server.lua oyundan atmasin)
            Task.Run(() =>
            {
                try
                {
                    using (var webClient = new WebClient())
                    {
                        var durum = webClient.DownloadString($"{ServerCheckURL}?steamid={_steamHex}");
                        if (durum == "-4")
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

                            webClient.UploadString($"{ServerUpdateURL}?steamid={_steamHex}&durum=0", "POST", $"steamid={_steamHex}&durum=0");
                            Dispatcher.Invoke(delegate { BtnLaunch.IsEnabled = true; });
                        }
                        else
                        {
                            webClient.UploadString($"{ServerUpdateURL}?steamid={_steamHex}&durum={durum}", "POST", $"steamid={_steamHex}&durum={durum}");
                        }
                    }
                }
                catch
                {
                    // ignored
                }
            });

            // Online sayısını güncelle
            if (!string.IsNullOrEmpty(_updateObject.ServerCode))
            {
                Task.Run(() =>
                {
                    try
                    {
                        using (var webClient = new WebClient())
                        {
                            webClient.DownloadStringTaskAsync(new Uri($"https://servers-frontend.fivem.net/api/servers/single/{_updateObject.ServerCode}"))
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
            }

            // Hile isimlerini çek ve kill et
            Task.Run(() =>
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadStringTaskAsync(new Uri(LauncherUpdateURL)).ContinueWith(task =>
                    {
                        _updateObject = JsonConvert.DeserializeObject<UpdateObject>(task.Result);

                        var processes = Process.GetProcesses();
                        foreach (var process in processes)
                        {
                            if (_updateObject.Cheats.Any(s => s.Equals(process.ProcessName, StringComparison.OrdinalIgnoreCase)))
                            {
                                // Hileyi kapat
                                process.Kill();
                                _hile++;
                            }
                            else
                            {
                                _kontrolEdilen++;
                            }
                        }

                        // Hileye rastlanmışsa fivem kapat
                        if (_hile > 0)
                        {
                            // TODO: Hile report olacak

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
                        }
                    });
                }
            });
        }

        private Player GetSteamPlayer(string steamid)
        {
            // TODO: Task yapılacak

            try
            {
                string response;
                using (var webClient = new WebClient())
                {
                    response = webClient.DownloadString($"{SteamProxyURL}?id={steamid}");
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
            Process.Start(_updateObject.Discord);
        }

        private void btnTeamspeak_Click(object sender, RoutedEventArgs e)
        {
            Process.Start($"ts3server://{_updateObject.Teamspeak3}");
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
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
                    var r = webClient.UploadString($"{ServerUpdateURL}?steamid={_steamHex}&durum=1", "POST", $"steamid={_steamHex}&durum=1");
                    if (r == "1")
                    {
                        BtnLaunch.IsEnabled = false;
                        var processName = $"fivem://connect/{(_isLocal ? "localhost:30120" : _updateObject.Server)}";

                        Task.Run(() =>
                        {
                            var process = Process.Start(processName);
                            process?.WaitForExit();
                        }).ContinueWith(task =>
                        {
                            Dispatcher.Invoke(delegate { BtnLaunch.IsEnabled = true; });
                        });
                    }
                    else
                    {
                        MessageBox.Show("Sunucu kaydınız yapılamadı. Daha sonra tekrar deneyin.");
                    }
                }
            }
            catch (Exception err)
            {
                MessageBox.Show($"Hata:{err.Message}");
            }
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            MouseLeftButtonDown += delegate { DragMove(); };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!BtnLaunch.IsEnabled)
            {
                e.Cancel = MessageBox.Show($"Launcher kapatırsanız, Fivem de kapanacak.{Environment.NewLine}Emin misiniz?", "GormYa Launcher", MessageBoxButton.YesNo, MessageBoxImage.Question) != MessageBoxResult.Yes;
                return;
            }

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
                    webClient.UploadString($"{ServerUpdateURL}?steamid={_steamHex}&durum=0", "POST", $"steamid={_steamHex}&durum=0");
                }
            }
            catch
            {
                // ignored
            }
        }
    }
}
