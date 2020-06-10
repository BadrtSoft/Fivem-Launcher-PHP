# Fivem-Launcher  
  
**Kurulum**
- C# projesinde MainWindow.xaml.cs dosyasindaki 4 adet linki kendi sunucunuza gore degistirin  
- C# projesinde dilerseniz background image degistirin ve Release modda derleyin  
- PHP klasorundeki guncelle.php ve kontrol.php dosyalarinda veritabani bilgilerini girini cloudflare ve whitelist ayarlarini yapin  
- PHP klasorundeki update.php dosyasina sunucu bilgilerini girin  
- PHP klasorundeki steamProxy.php dosyasina Steam API anahtarini girin  
- PHP dosyalarini Fivem sunucunuzda ilgili yerlere atabilirsiniz veya ayri bir hosting uzerinde barindirabilirsiniz  
- LauncherKontrol klasorundeki server.lua dosyasi icerisindeki linkleri kendi PHP sunucunuza gore ayarlayin  
- LauncherKontrol klasorunu resources klasorunuze kopyalayin ve config dosyasindan start verin  
  
**Yapılacaklar Yol Haritası:**  
- SteamKit2 kodlarında kullanılmayan özellikler ve değişkenler silinecek  
- Guncelle.php kaldırılıp, web socket server yapılması, C# web socket client yapılması ile birlikte launcher bypass engellenmesi  
- C# tarafında hile programlarını algılamak için kelime bazlı aramaya ek olarak sezgisel algılama algoritmasının yazılması  
- PHP tarafında discord webhook bildirimleri için fonksiyon oluşturulması  
- Değişkenlerin yönetimi için web panel  
- C# tarafına duyuru gönderilmesi için web panel  
