<?php
  include('../assets/includes/master.php');
?>
<html>
  <head>
    <noscript>
      <meta HTTP-EQUIV="REFRESH" content="0; url=https://eugenel.in/noscript">
    </noscript>
    <title>Leaderboard - <?php echo $site_title; ?></title>
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <link type="text/css" rel="stylesheet" href="../assets/css/materialize.css?<?php if ($reload_assets) { echo time(); } ?>"  media="screen,projection">
    <link type="text/css" rel="stylesheet" href="../assets/css/main.css?<?php if ($reload_assets) { echo time(); } ?>">
    <script src="https://code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="https://use.fontawesome.com/4857764df8.js"></script>
    <script src='https://www.google.com/recaptcha/api.js'></script>
    <script src="../assets/js/materialize.js?<?php if ($reload_assets) { echo time(); } ?>"></script>
    <script src="../assets/js/jquery.md5.js?<?php if ($reload_assets) { echo time(); } ?>"></script>
    <script>
    </script>
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
  </head>
  <body class="grey darken-4">
    <script>
      $(function() {
        $(".button-collapse").sideNav();
      });
      $(window).load(function() {
        setTimeout(function() {
          $('.preloader').fadeOut(1000, 'swing', function(){});
        }, 500);
        loadEntries('1'); // load  the first entries
      });
      function loadEntries(index, caller) {
        $.ajax({ // standard ajax setup
          type: "GET",
          url: "../api/fetch_scores.php",
          data: "index=" + index,
          success: function(data) {
            var obj = JSON.parse(data); // parse to object, this is why I use json over xml
            if (obj.status == 1) { // if success
              $("#holder table").remove(); // clear the table
              if (caller != null) { // if called from an actual click event
                $(".pagination .active").removeClass("active"); // remove active marker
                $(caller).parent().addClass("active"); // set the parent as active
              }
            }
            $("#holder").append("<table white-text><tr white-text><th>Username</th><th>Score</th><th>Date</th></tr>"); // set the heading
            $.each(obj.content.entries, function(index, value) { // iterate through each item
              var entryHtml = "<tr>";
              entryHtml += "<td>" + value.username + "</td>";
              entryHtml += "<td>" + value.score + "</td>";
              entryHtml += "<td>" + value.time + "</td>";
              entryHtml += "</tr>"; // build table row
              $("#holder").append(entryHtml); // add to table
            });
            $("#holder").append("</table>"); // close the table
          }
        })
      }
    </script>
    <div class="preloader">
    </div>
    <nav>
      <div class="nav-wrapper">
<?php
  $raw_sites = file_get_contents('../assets/json/navbar.json'); // load the json
  $decoded_sites = json_decode($raw_sites); // decode it
  echo "        ".$decoded_sites->logo."\n"; // get the "logo"
  $normal_links = array(); // create an array for links on larger screens
  $mobile_links = array(); // create the same array for mobile compatibility
  foreach ($decoded_sites->navbar as $single_site) { // iterates through each site
    if ($single_site->hide) { // ignores if set to be hidden
      continue;
    }
    $url = $single_site->link; // gets the link
    $fa = $single_site->fa; // gets the font awesome icon
    $name = $single_site->name; // gets the name of the site
    $normal_active = ""; // set activity
    $mobile_active = ""; // same
    if ($single_site->cwd == getcwd()) { // if this is the current page due to directories
      $url = "#!"; // set to not change page
      $normal_active = " active"; // set the active class for desktop devices
      $mobile_active = " class=\"active\""; // same for mobile devices
    }
    array_push($normal_links, "<li class=\"waves-effect waves-lighten$normal_active\"><a href=\"$url\"><i class=\"fa fa-fw fa-$fa\"></i>&nbsp; $name</a></li>"); // push to array for normal devices
    array_push($mobile_links, "<li$mobile_active><a href=\"$url\">$name</a></li>"); // same for mobile
  }
?>
        <a href="#" data-activates="mobile-nav" class="button-collapse right"><i class="material-icons">menu</i></a>
        <ul class="right hide-on-med-and-down">
<?php
  $normal_padding = "          "; // the actual padding to make the resulting html look nice
  foreach ($normal_links as $normal_link) { // iterates through each normal link
    echo $normal_padding.$normal_link."\n"; // prints it with the padding
  }
?>
        </ul>
        <ul class="side-nav" id="mobile-nav">
<?php
  $mobile_padding = "          "; // same as abovey for mobile
  foreach ($mobile_links as $mobile_link) {
    echo $mobile_padding.$mobile_link."\n";
  }
?>
        </ul>
      </div>
    </nav>
    <main class="white-text">
      <div class="container">
        <h4>Leaderboards</h4>
        <table id="holder" style="width:100%">
        </table>
        <ul class="pagination">
<?php
  $pagination_padding = "          "; // the preset padding
  $entry_result = $db->query("SELECT * FROM scores"); // fetch all rows from the database
  $entry_count = mysqli_num_rows($entry_result); // get the number of rows
  $page_count = ceil($entry_count / $entries_per_page); // get the number of pages to display all entries
  if ($page_count > $max_pages) { // if there are more pages than the maximum
    $page_count = $max_pages; // limit the page number so as to not fill up the page
  }
  for ($i = 1; $i <= $page_count; $i++) { // notice the modified for loop to allow for 1-indexed numbers
    echo $pagination_padding."<li class=\"waves-effect";
    if ($i == 1) { // if it's the first one
      echo " active"; // mark as active
    }
    echo "\"><a onclick=\"loadEntries('".strval($i)."', this)\">".strval($i)."</a></li>\n"; // fill with index
  }
?>
        </ul>
      </div>
    </main>
    <footer class="page-footer">
<?php
  // this is the same code as the navbar but for the page footer
  $raw_footer = file_get_contents('../assets/json/footer.json');
  $decoded_footer = json_decode($raw_footer);
  $footer_links = array();
  $footer_title = $decoded_footer->title;
  $footer_desc = $decoded_footer->description;
  $footer_copyright = $decoded_footer->copyright;
  $footer_made = $decoded_footer->madewith;
  foreach ($decoded_footer->links as $single_link) {
    if ($single_link->hide) {
      continue;
    }
    $link_name = $single_link->name;
    $link_fa = $single_link->fa;
    $link_url = $single_link->link;
    array_push($footer_links, "<li><a class=\"grey-text text-lighten-3\" href=\"$link_url\"><i class=\"fa fa-fw fa-$link_fa\"></i>&nbsp; $link_name</a></li>");
  }
?>
      <div class="container">
        <div class="row">
          <div class="col l6 s12">
            <h5 class="white-text"><?php echo $footer_title; ?></h5>
            <p class="grey-text text-lighten-4"><?php echo $footer_desc; ?></p>
          </div>
        <div class="col l4 offset-l2 s12">
          <h5 class="white-text">Links</h5>
            <ul>
<?php
  $footer_padding = "              ";
  foreach ($footer_links as $single_footer_link) {
    echo $footer_padding.$single_footer_link."\n";
  }
?>
            </ul>
          </div>
        </div>
      </div>
      <div class="footer-copyright">
        <div class="container row">
          <div class="col l6 s12">
            <span>Copyright &copy; <?php echo date("Y"); ?> <?php echo $footer_copyright; ?></span>
          </div>
          <div class="col l4 offset-l2 s12">
            <span><?php echo $footer_made; ?></a></span>
          </div>
        </div>
      </div>
    </footer>
  </body>
</html>
