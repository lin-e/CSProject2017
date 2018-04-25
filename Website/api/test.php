<?php
  include("../assets/includes/master.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->token)) { // if captcha is empty
    die("{\"status\":0,\"content\":\"Please provide an authorisation token\"}");
  } else {
    $token = $data->token;
    $result = validate_token($token);
    if ($result["status"] == 1) {
      $new_token = $result["content"];
      die("{\"status\":1,\"content\":\"$new_token\"}");
    } else {
      $content = $result["content"];
      die("{\"status\":0,\"content\":\"$content\"}");
    }
  }
  die("{\"status\":0,\"content\":\"Unknown error\"}");
?>