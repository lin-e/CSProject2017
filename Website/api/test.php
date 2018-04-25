<?php
  include("../assets/includes/config.php"); // include configuration (which includes database connection)
  include("../assets/includes/functions.php"); // include functions file
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
      $token_check = $db->query("SELECT * FROM user_sessions WHERE sessionid='$token' AND active=1") or die("{\"status\":0,\"content\":\"Invalid token\"}"); // query for token - must check that the token is also still active
      if (mysqli_num_rows($token_check) == 0) { // if doesn't exist
        die("{\"status\":0,\"content\":\"Invalid token\"}");
      }
      $row = $token_check->fetch_assoc(); // fetch row
      $ip = $_SERVER['REMOTE_ADDR']; // get the ip address of the user
      if ($ip == strval($row["ip"])) { // if the ip is the same as the original login
        $current_time = time();
        if ($current_time > intval($row["expire_time"])) {
          $db->query("UPDATE user_sessions SET active=0, expire_reason='EXPIRED' WHERE sessionid='$token'"); // invalidate the session
          die("{\"status\":0,\"content\":\"Token expired\"}"); // expired error
        } else {
          $username = strval($row["username"]); // get username
          $expire = strval($current_time + $token_lifetime_extension); // add time
          $db->query("UPDATE user_sessions SET expire_time=$expire WHERE sessionid='$token'"); // extend the expire time of the token
          die("{\"status\":1,\"content\":\"{\"user\":\"$username\",\"expire\":$expire}\"}"); // output username and expire time
        }
      } else {
        $db->query("UPDATE user_sessions SET active=0, expire_reason='IP_CHANGE' WHERE sessionid='$token'"); // invalidate the session
        die("{\"status\":0,\"content\":\"Invalid token\"}"); // give a generic error to attackers
      }
    } else {
      die("{\"status\":0,\"content\":\"Invalid token\"}");
    }
  }
  die("{\"status\":0,\"content\":\"Unknown error\"}");
?>