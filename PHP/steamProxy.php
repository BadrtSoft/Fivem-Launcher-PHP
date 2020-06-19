<?PHP
require 'ayarlar.php';

if (empty($_GET['id'])){
	die('{ "error": "id bos olamaz" }');
}

$steamid = $_GET['id'];

$url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=' . $steam_api_key . '&steamids=' . $steamid;

$contents = file_get_contents($url);

if($contents !== false){
    echo $contents;
} else {
	if (!function_exists('curl_init')) {
        die('{ "error": "sunucuda curl yuklu degil" }');
	}
	
    $ch = curl_init();
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);
    $contents = curl_exec($ch);
	echo $contents;
}
?>