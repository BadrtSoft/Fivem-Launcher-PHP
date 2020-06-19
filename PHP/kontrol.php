<?php
require 'ayarlar.php';

if (empty($_GET['steamid'])){
	die("-2");
}

$steamid = $_GET['steamid'];

if (!empty($_GET['ipv4'])){
	$ip = $_GET['ipv4'];
}

$conn = new mysqli($db_addr, $db_user, $db_pass, $db_name);
if (mysqli_connect_errno()) {
	die("-2");
} else {
	$olderDate = new DateTime();
	$olderDate->add(DateInterval::createFromDateString('-1 minute'));
	
	$query = $conn->prepare("UPDATE LauncherStatuses SET status=0 WHERE status!=-5 AND login_date < '" . $olderDate->format('Y-m-d H:i:s') . "'");
	$query->execute();
	$query->close();
	
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
				echo "0";
			}
		}
		else{
			echo $status;
		}
	}

	mysqli_close($conn);	
}
?>