<?php
// studenthome.hku.nl/~bob.hoogenboom/getBirthdayUser.php?uname=tester123&sid=5mm2sujfl121h7vv6c6fd7jglc
ini_set('display_errors', '1');
ini_set('display_startup_errors', '1');
error_reporting(E_ALL);

include 'connect.php';

// Input ophalen en Sanitizen (htmlspecialchars)   
if (!isset($_GET['uname'])|| !isset($_GET['sid'])){
    echo json_encode(["Variable not found" => 0]);
    exit;
}

$userName = htmlspecialchars($_GET['uname'] ?? null);
$sessionID = htmlspecialchars($_GET['sid'] ?? null); 

// Mist een input? Exit
if (!$userName || !$sessionID) {
    echo json_encode(["Value not found" => 1]);
    exit;
}

// Query voorbereiden
$stmt = $mysqli->prepare("SELECT geboortedatum FROM users WHERE nickname = ?");
$stmt->bind_param("s", $userName);
$stmt->execute();
$result = $stmt->get_result();

if ($row = $result->fetch_assoc()) {
    $geboortedatum = $row["geboortedatum"];
    $timestamp = strtotime($geboortedatum);

    // Format: "den 27e dag van het jaar 2025"
    echo date("d-m-Y", $timestamp); // or any format you prefer

    // Example custom format:
    // echo date("\d\e\\n jS \d\a\g \v\a\\n \h\e\\t \j\a\a\\r Y", $timestamp);
    echo date("\d\e\\n j\e \d\a\g \v\a\\n \h\e\\t \j\a\a\\r Y", $timestamp);

    
} else {
    echo json_encode(["Nog lang niet jarig" => 0]);
}
?>