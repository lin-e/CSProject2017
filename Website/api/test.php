<?php
  include("../assets/includes/master.php"); // include configuration (which includes database connection)
  $conn = new PDO("mysql:host=$db_hostname;dbname=$db_name", $db_username, $db_password);
  $result = $conn->prepare("SELECT * FROM users WHERE username= :username");
  $uname = 'authtest';
  $result->bindParam(':username', $uname);
  $result->execute();
  var_dump($result->fetch(PDO::FETCH_ASSOC));
?>