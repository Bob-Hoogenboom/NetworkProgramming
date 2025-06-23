<?php
// studenthome.hku.nl/~bob.hoogenboom/serverLogin.php? (result 0)
// studenthome.hku.nl/~bob.hoogenboom/serverLogin.php?id=1&pw=DitIsServer01!

//als je de sessie eventjes niet gebruikt dan eindigd de sessie 

ini_set('display_errors', '1');
ini_set('display_startup_errors', '1');
error_reporting(E_ALL);

//TODO sessie ID in een variabel in Unity opslaan en gebruiken voor score opslaan op database

include 'connect.php';

// Input ophalen en Sanitizen (htmlspecialchars)   
if (!isset($_GET['id'])
|| !isset($_GET['pw']))
{
    echo json_encode(["Variable not found" => 0]);
    exit;
}

$server_id = htmlspecialchars($_GET['id'] ?? null);   
$password = htmlspecialchars($_GET['pw'] ?? null);      

// Mist een input? Exit
if (!$server_id || !$password) {
    echo json_encode(["result" => 0]);
    exit;
}

session_start();

// Query uitvoeren
$stmt = $mysqli->prepare("SELECT password FROM gdv_servers WHERE id = ?");
$stmt->bind_param("i", $server_id);
$stmt->execute();
$result = $stmt->get_result();

if ($row = $result->fetch_assoc()) {
    if ($row['password'] === $password) {
        $_SESSION['server_id'] = $server_id; //sla ID op in de sessie
        echo json_encode(["result" => session_id()]);
    } else {
        echo json_encode(["result" => 0]);
        exit;
    }
} else {
    echo json_encode(["result" => 0]);
    exit;
}
?>
