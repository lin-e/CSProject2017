<?php
  include("../assets/includes/master.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->token)) { // if token is empty
    die("{\"status\":0,\"content\":\"Please provide an authorisation token\"}"); // error for authorisation
  } else {
    $token = $data->token; // extract the token from the input json
    $result = validate_token($token); // validate the token from the master function
    if ($result["status"] == 1) { // if the result is a success
      $new_token = $result["content"]; // create a new token
      die("{\"status\":1,\"content\":\"$new_token\"}"); // die with the new content
    } else { // if failed
      $content = $result["content"]; // set the content to be the validation content
      die("{\"status\":0,\"content\":\"$content\"}"); // die with the new content
    }
  }
  die("{\"status\":0,\"content\":\"Unknown error\"}"); // it really shouldn't reach this line, generic error
?>