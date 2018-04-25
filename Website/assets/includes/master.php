<?php
  include("config.php"); // config includes secret

  // TOKEN REASONS:
  // EXPIRED - the token was used after the expiry time
  // NEW_AUTH - event happened due to user's manual authentication
  // IP_CHANGE - the token was used by a different IP than the one used to create the token
  // TOKEN_RENEWAL - event happened due to automatic renewal

  // FUNCTIONS
  function generate_token_string($len) { // function to generate a token
    $characters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789"; // the available characters for the token
    $final = ""; // the string to return
    for ($i = 0; $i < $len; $i++) { // iterate for the set length
      $final .= $characters[rand(0, strlen($characters) - 1)]; // append to final string
    }
    return $final; // return the final string
  }
  function generate_token() { // moreso a function to check whether the generated token can be used
    while (true) { // iterate until token generated
      $generated = generate_token_string($token_length); // generate token
      $sessionid_check = $db->query("SELECT * FROM user_sessions WHERE sessionid='$generated'"); // query to fetch token with generated id
      if (mysqli_num_rows($sessionid_check) == 0) { // if id isn't in use
        return $generated; // exit loop
      }
    }
    return null; // if for some reason it doesn't work, give a default return value
  }
  function invalidate_token($token, $reason) { // function to mark a token as invalid
    $flag = true; // set return flag
    $db->query("UPDATE user_sessions SET active=0, expire_reason='$reason' WHERE sessionid='$token'") or $flag = false; // mark flag as false if the query fails
    return $flag; // return the flag
  }
  function create_token($user, $reason, $invalidate) {
    $flag = true; // same as invalidation flag
    $token = generate_token(); // generate the token string
    $time_string = strval(time()); // get the current timestamp
    $expire = strval(time() + $token_lifetime_extension); // set expire time
    $ip = $_SERVER['REMOTE_ADDR']; // get the ip address of the user
    if ($invalidate) { // if the function should handle token invalidation (small optimisation to not have to fetch from database if we already know the token)
      $existing_token = $db->query("SELECT * FROM user_sessions WHERE username='$user' AND active=1"); // find the existing token
      if (!(mysql_num_rows($existing_token) == 0)) { // if there is an active session
        $row = $existing_token->fetch_assoc(); // get the row
        invalidate_token(strval($row["sessionid"]), $reason) or $flag = false; // invalidate with the same reason for consistency
        if (!$flag) { // if error
          return false; // raise an error
        }
      }
    }
    $db->query("INSERT INTO user_sessions (sessionid, username, ip, time_started, expire_time, active, create_reason) VALUES ('$token', '$user', '$ip', $time_string, $expire, 1, '$reason')") or $flag = false; // create a new session or error out
    if (!$flag) { // same flagging code to pass errors
      return false;
    }
    return $token; // return the generated token
  }
  function validate_token($token) { // token validation
    $return_val = array(); // holds the return value
    $return_val["status"] = 0; // this is the most common return value, so we can use it as the default
    $return_val["content"] = "Invalid token";
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
        return $return_val;
      }
      $token_check = $db->query("SELECT * FROM user_sessions WHERE sessionid='$token' AND active=1") or die("{\"status\":0,\"content\":\"Invalid token\"}"); // query for token - must check that the token is also still active
      if (mysqli_num_rows($token_check) == 0) { // if doesn't exist
        return $return_val;
      }
      $row = $token_check->fetch_assoc(); // fetch row
      $ip = $_SERVER['REMOTE_ADDR']; // get the ip address of the user
      if ($ip == strval($row["ip"])) { // if the ip is the same as the original login
        $current_time = time();
        if ($current_time > intval($row["expire_time"])) {
          invalidate_token($token, "EXPIRED"); // invalidate the session
          $return_val["content"] = "Token expired"; // change the error message
          return $return_val; // return the item
        } else {
          $flag = true; // error flag
          $username = strval($row["username"]); // get username
          invalidate_token($token, "TOKEN_RENEWAL", false) or $flag = false; // mark the old token as invalidated due to a new token being generated automatically, instead of by a user auth event
          if ($flag) {
            $generated = create_token($username, "TOKEN_RENEWAL") or $flag = false; // generate a new token or error out
            if ($flag) {
              $return_val["status"] = 1; // mark as success
              $return_val["content"] = $generated; // set the return content as the token
            }
          }
          return $return_val;
        }
      } else {
        invalidate_token($token, "IP_CHANGE"); // invalidate the session
        return $return_val; // give a generic error to attackers
      }
    } else {
      return $return_val;
    }
  }
?>