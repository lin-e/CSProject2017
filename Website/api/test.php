<?php
  include("../assets/includes/config.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->token)) { // if captcha is empty
    die("{\"status\":0,\"content\":\"Please provide an authorisation token\"}");
  } else {
    $token = $data->token;
    if (strlen($token) == $token_length) {
      $allowed_chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // pull allowed characters from config
      $valid = true; // flag for name validity
      for ($i = 0; $i < strlen($token); $i++) { // iterate through each character in string
        $char = strval($token[$i]); // get character as string
        if (!(strpos($allowed_chars, $char) !== false)) { // check if not contained in allowed characters
          $valid = false; // mark as false
          break; // exit loop
        }
      }
      if (!$valid) { // if invalid
        die("{\"status\":0,\"content\":\"Invalid token\"}");
      }
      $token_check = $db->query("SELECT * FROM user_sessions WHERE sessionid='$token'") or die("{\"status\":0,\"content\":\"Invalid token\"}"); // query for token
      if (mysqli_num_rows($token_check) == 0) { // if doesn't exist
        die("{\"status\":0,\"content\":\"Invalid token\"}");
      }
      $row = $token_check->fetch_assoc(); // fetch row
      $username = strval($row["username"]); // get username
      die("{\"status\":1,\"content\":\"$username\"}"); // output username
    } else {
      die("{\"status\":0,\"content\":\"Invalid token\"}");
    }
  }
  die("{\"status\":0,\"content\":\"Unknown error\"}");
?>