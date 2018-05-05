<?php
  include("../assets/includes/master.php"); // include configuration (which includes database connection)
  $result = $db->prepare("SELECT * FROM users WHERE username= :username");
  $result->bindParam(':username', 'authtest');
  $result->execute();
  var_dump($result->fetch(PDO::FETCH_ASSOC));
?>