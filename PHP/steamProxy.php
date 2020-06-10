<?PHP
$steamApiKey = 'STEAM API KEY YAZINIZ';
$url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=' . $steamApiKey . '&steamids=' . $_GET['id'];
 
$contents = file_get_contents($url);

if($contents !== false){
    echo $contents;
}

?>