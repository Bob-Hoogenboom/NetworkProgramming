<?php
ini_set('display_errors', '1');
ini_set('display_startup_errors', '1');
error_reporting(E_ALL);

include 'connect.php';

if ($mysqli->connect_errno) {
    echo json_encode(["error" => "Failed to connect to MySQL: " . $mysqli->connect_error]);
    exit;
}

// Fetch top score
$query = "SELECT u.nickname, s.score, s.achievedate
FROM gdv_scores s
LEFT JOIN users u ON s.playerid = u.id
ORDER BY s.score DESC, s.achievedate ASC
LIMIT 1";

$result = $mysqli->query($query);

if (!$result) {
    echo json_encode(["error" => "Query failed: " . $mysqli->error]);
    exit;
}

$row = $result->fetch_assoc();

if ($row) {
    echo json_encode([
        "nickname" => $row['nickname'],
        "score" => (int)$row['score'],
        "date" => $row['achievedate']
    ]);
} else {
    echo json_encode(["message" => "No scores found."]);
}
?>