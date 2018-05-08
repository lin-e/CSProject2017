<?php
  include("../assets/includes/master.php"); // include configuration (which includes database connection)
  if (isset($_GET["index"])) { // if the param is set
    if (ctype_digit($_GET["index"])) { // if all numerical
      $return_val = array(); // we're returning a json string, hence we'll need to make an object array
      $return_val["status"] = 1; // mark as success
      $return_val["content"] = array(); // create a new array to hold records
      $return_val["content"]["count"] = 0; // count for the number of entries found
      $return_val["content"]["entries"] = array(); // holds records here
      $index = intval($_GET["index"]) - 1; // set the index to be one less than the index specified (we're using a 1-indexed selector)
      if ($index < 0) { // if the index is negative
        die("{\"status\":0,\"content\":\"Invalid index\"}"); // die with error
      }
      $lower_limit = strval($index * $entries_per_page); // get the lower limit for SQL
      $rows = $db->query("SELECT * FROM scores ORDER BY score ASC LIMIT $lower_limit, ".strval($entries_per_page)) or die("{\"status\":0,\"content\":\"Database error\"}"); // query the database with the limits or error out
      while ($row = $rows->fetch_assoc()) { // while there are unread rows
        $new_entry = array(); // create a new array to hold content
        $new_entry["score"] = intval($row["score"]); // pull the score from the database
        $new_entry["username"] = strval($row["username"]); // same as above but for username
        $time = intval($row["logtime"]); // get the time from the row
        $time_str = date("jS \o\\f F Y", $time); // convert from timestamp to time
        $new_entry["time"] = $time_str; // set the tiem string in the return array
        array_push($return_val["content"]["entries"], $new_entry); // add to the end of the array
      }
      $return_val["content"]["count"] = count($return_val["content"]["entries"]); // set the count
      die(json_encode($return_val)); // returns the entire array content
    } else {
      die("{\"status\":0,\"content\":\"Invalid index\"}"); // die with error
    }
    die("{\"status\":0,\"content\":\"Please supply an index\"}"); // die with error
  }
?>