# Fivem-Launcher  
  
**Özellikleri**  
- Launcher çalıştığı zaman hile programlarını kapatır. 25 saniyede bir düzenli olarak kontrol eder.  
- Launcher kapandığında, Fivem'i de otomatik olarak kapatır.  
- Steam'in açık olup olmadığını kontrol eder. Steam bilgilerini okur.  
- Sunucu içerisindeki online oyuncu sayısını gösterir.  
- Otomatik güncelleme özelliği var. Oyunculara uzaktan güncelleme gönderebilirsiniz.  
- Discord ve Teamspeak3 linklerinizi uzaktan yönetebilirsiniz.  
- Renkli haritayı oyuncu bilgisayarına otomatik kopyalar. Bu sayede zoom yapıldığındaki hata giderilir.  
- Kendi içerisinde whitelist barındırır. Dilerseniz bunu kullanabilir, dilerseniz kendi whitelist scriptinizi kullanabilirsiniz.  
- PHP dosyalarını Fivem oyun sunucunuzda da çalıştırabilirsiniz, ayrı bir hosting üzerinde de çalıştırabilirsiniz.  
- Oyuncuların ip adreslerini kayıt eder. PHP dosyaları cloudflare arkasında çalışsa da IP adresini doğru alır.  
  
  
**Kurulum**
- C# projesinde ***MainWindow.xaml.cs*** dosyasindaki 4 adet linki kendi sunucunuza gore degistirin  
- C# projesinde dilerseniz tasarimi degistirin ve Release modda derleyin  
- PHP klasorundeki ***ayarlar.php*** dosyasinda bulunan ayarlari degistirin  
- PHP dosyalarini Fivem sunucunuzda ilgili yerlere (XAMPP Apache için muhtemelen ***C:\xampp\htdocs*** klasörü) atabilirsiniz veya ayri bir hosting uzerinde barindirabilirsiniz  
- PHP klasorundaki ***LauncherStatuses.sql*** SQL dosyasi ile veritabani tablolarinizi olusturun  
- LauncherKontrol klasorundeki ***server.lua*** dosyasi icerisindeki linkleri kendi PHP sunucunuza gore ayarlayin  
- ***LauncherKontrol*** klasorunu ***resources*** klasorunuze kopyalayin ve *config* dosyanizdan start verin  
  
  
**Yapılacaklar Yol Haritası:**  
- SteamKit2 kodlarında kullanılmayan özellikler ve değişkenler silinecek  
- Guncelle.php kaldırılıp, web socket server yapılması, C# web socket client yapılması ile birlikte launcher bypass engellenmesi  
- C# tarafında hile programlarını algılamak için kelime bazlı aramaya ek olarak sezgisel algılama algoritmasının yazılması  
- PHP tarafında discord webhook bildirimleri için fonksiyon oluşturulması  
- Değişkenlerin yönetimi için web panel  
- C# tarafına duyuru gönderilmesi için web panel  
  
  
Discord: [https://discord.gg/e9URnEe](https://discord.gg/e9URnEe)  
