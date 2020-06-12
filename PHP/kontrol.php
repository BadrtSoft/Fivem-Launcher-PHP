<?php
require 'ayarlar.php';

if (empty($_GET['steamid'])){
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
			echo "0";
		}
	}
	else{
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