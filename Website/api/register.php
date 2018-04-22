<?php
  include("../assets/includes/config.php");
  $data = json_decode($_POST['data']);
  if (!isset($data->username)) {
    die("{\"status\":0,\"content\":\"No username specified\"}");
  }
  if (!isset($data->md5)) {
    die("{\"status\":0,\"content\":\"No password hash specified\"}");
  }
  die("{\"status\":1,\"content\":\"Success\"}")
?>