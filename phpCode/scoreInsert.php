<?php
// studenthome.hku.nl/~bob.hoogenboom/scoreInsert.php?score=73

ini_set('display_errors', '1');
ini_set('display_startup_errors', '1');
error_reporting(E_ALL);

include 'connect.php'; // Jouw databaseverbinding

session_start();

// Check of alle benodigde data aanwezig is
if (isset($_SESSION['user_id']) 
&& isset($_SESSION['server_id']) 
&& isset($_GET['score'])
&& filter_var($_GET['score'], FILTER_VALIDATE_INT)!== false
) {
    $user_id = intval($_SESSION['user_id']);
    $server_id = intval($_SESSION['server_id']);
    $score = intval($_GET['score']);

    $query = "INSERT INTO gdv_scores (id, gameid, playerid, serverid, score, achievedate) 
              VALUES (NULL, 1, '$user_id', '$server_id', '$score', NOW())";

    if ($mysqli->query($query)) {
        echo "Score succesvol opgeslagen!";
    } else {
        echo "Fout bij opslaan score: " . $mysqli->error;
    }
} else {
    echo "Vereiste gegevens ontbreken. Zorg dat je bent ingelogd als server én user, en geef een score mee.";
}
?>
