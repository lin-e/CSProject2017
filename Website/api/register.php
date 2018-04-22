<?php
  include("../assets/includes/config.php"); // include configuration (which includes database connection)
  $data = json_decode($_POST['data']); // take the post parameter and decode it
  if (!isset($data->username)) { // if the username isn't set in the json
    die("{\"status\":0,\"content\":\"No username specified\"}"); // die with error
  }
  if (!isset($data->md5)) { // if the hash isn't set
    die("{\"status\":0,\"content\":\"No password hash specified\"}");
  } else {
    $md5 = $data->md5; // take the variable
    if (!(strlen($md5) == 32 && ctype_xdigit($md5))) { // if length isn't 32 or contains non hex characters
      die("{\"status\":0,\"content\":\"Invalid hash\"}");
    }
  }
  die("{\"status\":1,\"content\":\"Success\"}")
?>