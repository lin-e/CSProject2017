<?php
  include("secret.php");
  // DATABASE SETTINGS
  $db_username = 'csproject'; // MySQL username
  $db_password = $sec_db_password; // MySQL password, pull from secret.php so sensitive data won't be published.
  $db_hostname = 'localhost'; // MySQL host
  $db_name = 'cs_project'; // MySQL datbase name
  $db = mysqli_connect($db_hostname, $db_username, $db_password, $db_name) or die("{\"status\":0,\"content\":\"Failed to connect to database\"}"); // connect or error

  $username_min_length = 4;
  $username_max_length = 32;
  $username_allowed_chars = 'abcdefghijklmnopqrstuvwxyz0123456789_-.';

  $reload_assets = false;
  $site_title = "project.eugenel.in";
?>
