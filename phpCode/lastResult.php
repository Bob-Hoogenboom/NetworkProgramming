<?php
include 'connect.php';

// Select the last added user (assuming 'id' is auto-incremented)
$query = "SELECT * FROM users ORDER BY id DESC LIMIT 1";

if (!($result = $mysqli->query($query))) {
    showerror($mysqli->errno, $mysqli->error); 
}

$my_json = "{\"users\":[";

if ($row = $result->fetch_assoc()) {
    $my_json .= json_encode($row);
}

$my_json .= "]}";

echo $my_json;
?>