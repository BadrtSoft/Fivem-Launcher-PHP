<?php
$steam_api_key  = ""; // Steam Dev. API key yazınız
$use_cloudflare = false; // PHP dosyalari cloudflare arkasinda calisacaksa (domain uzerinden) burayi true yapin

// Veritabani ile ilgili bilgiler
$db_addr = "127.0.0.1"; // mysql server adresi
$db_user = "root"; // mysql kullanici adi
$db_pass = ""; // mysql parolasi
$db_name = "essentialmode"; // mysql veritabani adi

// FiveM server ile ilgili bilgiler
$use_whitelist = false; // Whitelist icin LauncherStatuses tablosunu kullanacaksaniz burayi true yapin
$fivem_server  = "127.0.0.1:30120"; // fivem server ip adresi
$discord       = "https://discord.gg/e9URnEe"; // discord sunucu adresiniz (bos birakilirsa launcherda discord butonu gizlenir)
$teamspeak3    = ""; // teamspeak3 sunucu adresiniz (bos birakilirsa launcherda teamspeak butonu gizlenir)

// Launcher icerisindeki updater ile ilgili bilgiler
$exe_version = "2020.6.20.25"; // launcher sag tiklayip ozelliklerde yazan surum numarasi exe ile bu tutmadiginda update_file indirilip kurulur
$update_file = "https://yalc.in/fivem_launcher/update.zip"; // surum numarasi tutmadiginda indirilecek dosyanin adresi

// IP tespiti
$ip = $_SERVER['REMOTE_ADDR'];
if ($use_cloudflare && isset($_SERVER['HTTP_CF_CONNECTING_IP'])){
	$ip = $_SERVER['HTTP_CF_CONNECTING_IP'];
}
?>