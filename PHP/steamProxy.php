<?PHP

$steamApiKey = 'STEAM API KEY YAZINIZ';
$url = 'http://api.steampowered.com/ISteamUser/GetPlayerSummaries/v0002/?key=' . $steamApiKey . '&steamids=' . $_GET['id'];
 
//Once again, we use file_get_contents to GET the URL in question.
$contents = file_get_contents($url);
 
//If $contents is not a boolean FALSE value.
if($contents !== false){
    //Print out the contents.
    echo $contents;
}

?>