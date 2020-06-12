<?php
require 'ayarlar.php';

if (empty($_GET['steamid']) || !isset($_GET['durum'])){
	die("-2");
}

if ($_GET['durum'] != '-1' && $_GET['durum'] != '0' && $_GET['durum'] != '1'){
	die("-2");
}

$conn = new mysqli($db_addr, $db_user, $db_pass, $db_name);
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
			$query = $conn->prepare("INSERT INTO LauncherStatuses (`steamid`, `login_date`, `ip_address`, `status`) VALUES (?, NOW(), ?, ?)");
			$query->bind_param('sss', $_GET['steamid'], $ip, $_GET['durum']);
			$query->execute();
			$query->close();
			
			echo $_GET['durum'];
		}
	}
	else {
		if ($status == -1) { // eger oyundaysa ip adresini degistirmiyoruz ki, 2 kisi ayni anda giremesin
			$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), status=?");
			$query->bind_param('s', $_GET['durum']);
			$query->execute();
			$query->close();	
		} else {
			$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), ip_address=?, status=?");
			$query->bind_param('ss', $ip, $_GET['durum']);
			$query->execute();
			$query->close();
		}
		
		echo $_GET['durum'];
	}
}

mysqli_close($conn);
?>