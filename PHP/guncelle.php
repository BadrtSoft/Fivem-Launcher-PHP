<?php
require 'ayarlar.php';

if (empty($_GET['steamid']) || !isset($_GET['durum'])){
	die("-2");
}

if ($_GET['durum'] != '-5' && $_GET['durum'] != '-4' && $_GET['durum'] != '-1' && $_GET['durum'] != '0' && $_GET['durum'] != '1'){
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
		if ($status == -1 || $status == -4 || $status == 0 || $status == -5) {
			if ($status == -5) {
				die("-5");
			} else {
				if (!empty($_GET['cheat'])) {
					$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), status=?, cheat_name=? WHERE steamid=?");
					$query->bind_param('sss', $_GET['durum'], $_GET['cheat'], $_GET['steamid']);
					$query->execute();
					$query->close();
				} else {
					$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), status=? WHERE steamid=?");
					$query->bind_param('ss', $_GET['durum'], $_GET['steamid']);
					$query->execute();
					$query->close();
				}
			}
		} else {
			$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), ip_address=?, status=? WHERE steamid=?");
			$query->bind_param('sss', $ip, $_GET['durum'], $_GET['steamid']);
			$query->execute();
			$query->close();
		}
		
		echo $_GET['durum'];
	}
}

mysqli_close($conn);
?>