<?php
  include("../assets/includes/config.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->username)) { // if the username isn't set in the json
    die("{\"status\":0,\"content\":\"No username specified\"}"); // die with error
  } else {
    $username = strtolower($data->username);
    if (strlen($username) < $username_min_length || strlen($username) > $username_max_length) { // check username length
      die("{\"status\":0,\"content\":\"Invalid username\"}");
    } else {
      $allowed_chars = $username_allowed_chars; // pull allowed characters from config
      $valid = true; // flag for name validity
      for ($i = 0; $i < strlen($username); $i++) { // iterate through each character in string
        $char = strval($username[$i]); // get character as string
        if (!(strpos($allowed_chars, $char) !== false)) { // check if not contained in allowed characters
          $valid = false; // mark as false
          break; // exit loop
        }
      }
      if (!$valid) { // if invalid
        die("{\"status\":0,\"content\":\"Invalid username\"}");
      }
    }
  }
  if (!isset($data->md5)) { // if the hash isn't set
    die("{\"status\":0,\"content\":\"No password hash specified\"}");
  } else {
    $md5 = $data->md5; // take the variable
    if (!(strlen($md5) == 32 && ctype_xdigit($md5))) { // if length isn't 32 or contains non hex characters
      die("{\"status\":0,\"content\":\"Invalid hash\"}");
    }
  }
  $user_check = $db->query("SELECT * FROM users WHERE username='$username'"); // query the database for the password
  if (mysqli_num_rows($user_check) == 0) { // if no records
    die("{\"status\":0,\"content\":\"Login failed\"}");
  } else {
    $user = $user_check->fetch_assoc(); // get user row;
    if (password_verify($data->md5, strval($user["passhash"]))) { // check if the user's sent hash matches with the one stored, but using the built-in methods
      $generated = "";
      while (true) { // iterate until token generated
        $generated = generate_token($token_length); // generate token
        $sessionid_check = $db->query("SELECT * FROM user_sessions WHERE sessionid=$'$generated'");
        if (mysqli_num_rows($sessionid_check) == 0) { // if id isn't in use
          break; // exit loop
        }
      }
      $time_string = strval(time()); // get the current timestamp
      $expire= strval(time() + $token_lifetime_extension);
      $ip = $_SERVER['REMOTE_ADDR']; // get the ip address of the user
      $db->query("UPDATE user_sessions SET active=0 WHERE username='$username'") or die("{\"status\":0,\"content\":\"Login failed\"}"); // invalidate other sessions or fail
      $db->query("INSERT INTO user_sessions (sessionid, username, ip, time_started, expire_time, active) VALUES ('$generated', '$username', '$ip', $time_string, $expire, 1)") or die("{\"status\":0,\"content\":\"Login failed\"}"); // insert into sessions or die with error
      $db->query("UPDATE users SET sessionid='$generated' WHERE username='$username'") or die("{\"status\":0,\"content\":\"Login failed\"}"); // update user to get current token
      die("{\"status\":1,\"content\":\"$generated\"}"); // success; send token
    } else {
      die("{\"status\":0,\"content\":\"Login failed\"}");
    }
  }
  die("{\"status\":0,\"content\":\"Unknown error\"}");

  function generate_token($len) { // function to generate a token
    $characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // the available characters for the token
    $final = ""; // the string to return
    for ($i = 0; $i < $len; $i++) { // iterate for the set length
      $final .= $characters[rand(0, strlen($characters) - 1)]; // append to final string
    }
    return $final; // return the final string
  }
?>