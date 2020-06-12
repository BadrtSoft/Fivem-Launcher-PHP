<?PHP
require 'ayarlar.php';

if (!isset($_GET['id'])){
	die('{}');
}

$url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=' . $steam_api_key . '&steamids=' . $_GET['id'];
 
$contents = file_get_contents($url);

if($contents !== false){
    echo $contents;
}

?>