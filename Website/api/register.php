<?php
  include("../assets/includes/config.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->username)) { // if the username isn't set in the json
    die("{\"status\":0,\"content\":\"No username specified\"}"); // die with error
  } else {
    $username = strtolower($data->username);
    if (strlen($username) < $username_min_length || strlen($username) > $username_max_length) { // check username length
      die("{\"status\":0,\"content\":\"Username must be between ".strval($username_min_length)." and ".strval($username_max_length)." characters\"}");
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
        die("{\"status\":0,\"content\":\"Invalid characters in username; only alphanumeric characters, dots, dashes and underscores permitted\"}");
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
    $hashed = $db->real_escape_string(password_hash($data->md5, PASSWORD_DEFAULT)); // hash the string
    $db->query("INSERT INTO users (username, passhash) VALUES ('$username', '$hashed')") or die("{\"status\":0,\"content\":\"Error in user creation\"}"); // insert into database
  } else {
    die("{\"status\":0,\"content\":\"Username already exists\"}");
  }
  die("{\"status\":1,\"content\":\"Success\"}");
?>