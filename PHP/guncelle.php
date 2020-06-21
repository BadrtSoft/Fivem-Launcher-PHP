<?php
require 'ayarlar.php';

if (empty($_GET['steamid']) || empty($_GET['durum'])){
	die("-2");
}

$cheat = '';
$steamid = $_GET['steamid'];
$durum = $_GET['durum'];

if ($durum != '-5' && $durum != '-4' && $durum != '-1' && $durum != '0' && $durum != '1'){
	die("-2");
}

$date = date('Y-m-d H:i:s');

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
				$query = $conn->prepare("INSERT INTO LauncherStatuses (`steamid`, `login_date`, `ip_address`, `status`) VALUES (?, ?, ?, ?)");
				$query->bind_param('ssss', $steamid, $date, $ip, $durum);
				$query->execute();
				$query->close();
				
				echo $durum;
			}
		}
		else {
			if ($status == -1 || $status == -4 || $status == 0 || $status == -5) {
				if ($status == -5) {
					$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=? WHERE steamid=?");
					$query->bind_param('ss', $date, $steamid);
					$query->execute();
					$query->close();
					
					echo("-5");
				} else {
					if (!empty($_GET['cheat'])) {
						$cheat = $_GET['cheat'];
						$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=?, status=?, cheat_name=? WHERE steamid=?");
						$query->bind_param('ssss', $date, $durum, $cheat, $steamid);
						$query->execute();
						$query->close();
					} else {
						$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=?, status=? WHERE steamid=?");
						$query->bind_param('sss', $date, $durum, $steamid);
						$query->execute();
						$query->close();
					}
					
					echo $durum;
				}
			} else {
				$query = $conn->prepare("UPDATE LauncherStatuses SET login_date=?, ip_address=?, status=? WHERE steamid=?");
				$query->bind_param('ssss', $date, $ip, $durum, $steamid);
				$query->execute();
				$query->close();
				echo $durum;
			}
		}
	}

	mysqli_close($conn);
}
?>