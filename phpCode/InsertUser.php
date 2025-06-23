<?php

ini_set('display_errors', '1');
ini_set('display_startup_errors', '1');
error_reporting(E_ALL);

include 'connect.php';
//TODO Check of Sessie bestaat, na serverlogin*


// Input ophalen, check of uberhaubt de variabel bestaat met isset*
if (isset($_SESSION['user_id']) 
&& isset($_SESSION['server_id']) )
{

}

if (!$userName || !$password) {
    echo json_encode(["id" => 0]);
    exit;
}

// Query voorbereiden
$stmt = $mysqli->prepare("SELECT id, email, nickname, geboortedatum, password FROM users WHERE email = ?");
$stmt->bind_param("s", $userName);
$stmt->execute();
$result = $stmt->get_result();

if ($row = $result->fetch_assoc()) {
    if ($row['password'] === $password) { 
        echo json_encode([
            "id" => $row["id"],
            "email" => $row["email"],
            "Nick" => $row["nickname"],
            "dateOfBirth" => $row["geboortedatum"]
        ]);
    } else {
        echo json_encode(["id line 36" => 0]);
    }
} else {
    echo json_encode(["id line 39" => 0]);
}
?>