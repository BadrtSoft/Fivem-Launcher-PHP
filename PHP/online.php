<?php
require 'ayarlar.php';

$conn = new mysqli($db_addr, $db_user, $db_pass, $db_name);
if (mysqli_connect_errno()) {
    die("-2");
}

if ($stmt = $conn->prepare("SELECT steamid FROM LauncherStatuses WHERE status=-1")) {
	$stmt->execute();
	$stmt->store_result();
	$online = $stmt->num_rows;
	$stmt->close();
	
	echo $online;
}

mysqli_close($conn);
?>