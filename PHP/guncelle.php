<?php
require 'ayarlar.php';

if (empty($_GET['steamid']) || empty($_GET['durum'])){
	die("-2");
}

$steamid = $_GET['steamid'];
$durum = $_GET['durum'];

if ($durum != '-5' && $durum != '-4' && $durum != '-1' && $durum != '0' && $durum != '1'){
	die("-2");
}

$conn = new mysqli($db_addr, $db_user, $db_pass, $db_name);
if (mysqli_connect_errno()) {
	die("-2");
} else {
	if ($stmt = $conn->prepare("SELECT login_date, ip_address, status FROM LauncherStatuses WHERE steamid=? LIMIT 1")) {
		$stmt->bind_param("s", $steamid);
		$stmt->execute();
		$stmt->bind_result($login_date, $ip_address, $status);
		$stmt->fetch();
		$stmt->close();
		
		if (!isset($status)){
			if ($use_whitelist){
				echo "-3";
			} else {
				$query = $conn->prepare("INSERT INTO LauncherStatuses (`steamid`, `login_date`, `ip_address`, `status`) VALUES (?, NOW(), ?, ?)");
				$query->bind_param('sss', $steamid, $ip, $durum);
				$query->execute();
				$query->close();
				
				echo $durum;
			}
		}
		else {
			if ($status == -1 || $status == -4 || $status == 0 || $status == -5) {
				if ($status == -5) {
					echo("-5");
				} else {
					if (!empty($_GET['cheat'])) {
						$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), status=?, cheat_name=? WHERE steamid=?");
						$query->bind_param('sss', $durum, $_GET['cheat'], $steamid);
						$query->execute();
						$query->close();
					} else {
						$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), status=? WHERE steamid=?");
						$query->bind_param('ss', $durum, $steamid);
						$query->execute();
						$query->close();
					}
					
					echo $durum;
				}
			} else {
				$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=NOW(), ip_address=?, status=? WHERE steamid=?");
				$query->bind_param('sss', $ip, $durum, $steamid);
				$query->execute();
				$query->close();
				echo $durum;
			}
		}
	}

	mysqli_close($conn);
}
?>