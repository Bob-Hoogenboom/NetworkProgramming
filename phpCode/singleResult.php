<?php
include 'connect.php';

$query = "SELECT * FROM users"; //query

if (!($result = $mysqli->query($query))) // query toepassen
  showerror($mysqli->errno,$mysqli->error); 

  $row = $result->fetch_assoc(); //info uit "brei" halen

echo json_encode($row); //json laten zien
?>