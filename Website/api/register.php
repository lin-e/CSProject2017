<?php
  include("../assets/includes/config.php");
  $data = json_decode($_POST['data']);
  echo $data->foo;
?>