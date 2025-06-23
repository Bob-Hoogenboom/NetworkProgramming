<?php
include 'connect.php';

if(!isset($_GET[['gameid']])
||!isset($_GET[['playerid']])
||!isset($_GET[['serverid']])
||!isset($_GET[['score']])
||!isset($_GET[['sid']]))
{
    echo json_encode(["Variable not found" => 0]);
    exit;
}

$gameid = $_GET['gameid'] ?? null;
$playerid = $_GET['playerid'] ?? null;
$serverid = $_GET['serverid'] ?? null;
$score = $_GET['score'] ?? null;
$sessionID = htmlspecialchars($_GET['sid'] ?? null); //validate

// Mist een input? Exit
if (!$gameid || !$playerid || !$serverid || !$score || !$sessionID) {
    echo json_encode(["Value not found" => 1]);
    exit;
}

if (filter_var($gameid, FILTER_VALIDATE_INT)!== false   //validate
    && (filter_var($playerid, FILTER_VALIDATE_INT)!== false)    //validate
    && (filter_var($serverid, FILTER_VALIDATE_INT)!== false)    //validate
    && (filter_var($score, FILTER_VALIDATE_INT)!== false)   //validate
    ) 
    {
        
        echo("Variable is an integer");

    $query = "INSERT INTO gdv_scores (id, gameid, playerid, serverid, score) 
          VALUES (NULL, $gameid, $playerid, $serverid, $score)";

    if (!($result = $mysqli->query($query))) 
    {
        showerror($mysqli->errno, $mysqli->error); 
        exit;
    }
    
} else {
    echo("Variable is not an integer");
    exit;
}

//studenthome.hku.nl/~bob.hoogenboom/insertResult.php?gameid=2&playerid=2&serverid=2&score=2000
?>