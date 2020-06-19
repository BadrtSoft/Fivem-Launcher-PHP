<?php
require 'ayarlar.php';

$conn = new mysqli($db_addr, $db_user, $db_pass, $db_name);
if (mysqli_connect_errno()) {
    die("0");
} else {
	if ($stmt = $conn->prepare("SELECT steamid FROM LauncherStatuses WHERE status=-1")) {
		$stmt->execute();
		$stmt->store_result();
		$online = $stmt->num_rows;
		$stmt->close();
		
		echo $online;
	}else {
		echo "0";
	}
	
	mysqli_close($conn);	
}
?>