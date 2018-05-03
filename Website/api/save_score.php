<?php
  include("../assets/includes/master.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->token)) { // if token is empty
    die("{\"status\":0,\"content\":{\"token\":\"\",\"body\":\"Please provide an authorisation token\"}}"); // error for authorisation
  } else {
    $token = $data->token; // extract the token from the input json
    $result = validate_token($token); // validate the token from the master function
    if ($result["status"] == 1) { // if the result is a success
      $new_token = $result["content"]["token"]; // create a new token
      if (!isset($data->score)) { // if the score isn't set
        die("{\"status\":0,\"content\":{\"token\":\"$new_token\",\"body\":\"Please provide a score\"}}"); // die with token
        if (!is_int($data->score)) { // if the token is invalid
          die("{\"status\":0,\"content\":{\"token\":\"$new_token\",\"body\":\"Please provide a valid score\"}}"); // die with token
        }
      }
      $score = strval(intval($data->score)); // get the score as a string
      $time = strval(time()); // get the current time as a string
      $username = $result["content"]["username"]; // fetch the username
      $db->query("INSERT INTO scores (score, username, logtime) VALUES ($score, '$username', $time)") or die("{\"status\":0,\"content\":{\"token\":\"$new_token\",\"body\":\"Failed to insert\"}}"); // tell the user it failed, and provide a token
      die("{\"status\":1,\"content\":{\"token\":\"$new_token\",\"body\":\"Score saved\"}}"); // tell the user the score is saved
    } else { // if failed
      $content = $result["content"]; // set the content to be the validation content
      die("{\"status\":0,\"content\":{\"token\":\"\",\"body\":\"$content\"}}");
    }
  }
  die("{\"status\":0,\"content\":{\"token\":\"\",\"body\":\"Unknown error\"}}");
?>