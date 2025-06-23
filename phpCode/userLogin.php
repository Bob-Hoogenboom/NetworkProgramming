<?php
// studenthome.hku.nl/~bob.hoogenboom/userLogin.php? (result = 0)
// studenthome.hku.nl/~bob.hoogenboom/userLogin.php?uname=test@test.nl&pw=WachtwoordTest123!&sid=5mm2sujfl121h7vv6c6fd7jglc

ini_set('display_errors', '1');
ini_set('display_startup_errors', '1');
error_reporting(E_ALL);
//TODO vraag user info op via login screen in Unity*

include 'connect.php';

//TODO Check of Sessie bestaat, na serverlogin*
//if (isset($_GET['PHPSESSID'])) { //staat de sessie id in de url?

// Input ophalen en Sanitizen (htmlspecialchars)   
if (!isset($_GET['uname'])
|| !isset($_GET['pw'])
|| !isset($_GET['sid']))
{
    echo json_encode(["Variable not found" => 0]);
    exit;
}

$userName = htmlspecialchars($_GET['uname'] ?? null);   
$password = htmlspecialchars($_GET['pw'] ?? null);      
$sessionID = htmlspecialchars($_GET['sid'] ?? null); 

// Mist een input? Exit
if (!$userName || !$password || !$sessionID) {
    echo json_encode(["Value not found" => 1]);
    exit;
}

//sessie id voor deze sessie instellen naar wat uit url kwam
session_id($sessionID);
session_start();

//check actieve sessie
if (!isset($_SESSION['server_id'])) {
    echo json_encode(["No session active" => 0]);
    exit;
}

// Query voorbereiden
$stmt = $mysqli->prepare("SELECT id, email, nickname, geboortedatum FROM users WHERE email = ? AND password = ?");
$stmt->bind_param("ss", $userName, $password);
$stmt->execute();
$result = $stmt->get_result();

// Gebruiker gevonden?
if ($row = $result->fetch_assoc()) {
    echo json_encode([
        "id" => $row["id"],
        "email" => $row["email"],
        "Nick" => $row["nickname"],
        "dateOfBirth" => $row["geboortedatum"]
    ]);
} else {
    echo json_encode(["No Valid Login" => 0]); // geen match
    exit;
}
?>