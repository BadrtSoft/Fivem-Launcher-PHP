<?php
$servername     = "127.0.0.1"; // mysql server adresi
$username       = "root"; // mysql kullanici adi
$password       = "pass"; // mysql parolasi
$dbname         = "database"; // mysql veritabani adi
$use_cloudflare = false; // PHP dosyalari cloudflare arkasinda calisacaksa (domain uzerinden) burayi true yapin
$use_whitelist  = false; // Whitelist icin LauncherStatuses tablosunu kullanacaksaniz burayi true yapin

if (!isset($_GET['steamid'])){
	die("-2");
}

$conn = new mysqli($servername, $username, $password, $dbname);
if (mysqli_connect_errno()) {
    die("-2");
}

if ($stmt = $conn->prepare("SELECT login_date, ip_address, status FROM LauncherStatuses WHERE steamid=? LIMIT 1")) {
	$stmt->bind_param("s", $_GET['steamid']);
	$stmt->execute();
	$stmt->bind_result($login_date, $ip_address, $status);
	$stmt->fetch();
	$stmt->close();
	
	if (!isset($status)){
		if ($use_whitelist){
			echo "-3";
		} else {
			echo "0";
		}
	}
	else{
		$ip = $_SERVER['REMOTE_ADDR'];
		if ($use_cloudflare){
			$ip = $_SERVER['HTTP_CF_CONNECTING_IP'];
		}
		
		$date = new DateTime();
		$date2 = new DateTime($login_date);
		$seconds = $date->getTimestamp() - $date2->getTimestamp();
		
		if ($seconds > 60){
			echo "0";
		} else {
			if ($ip != $ip_address){
				echo "0";
			} else {
				echo $status;
			}
		}
	}
}

mysqli_close($conn);
?>