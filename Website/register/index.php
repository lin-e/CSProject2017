<?php
  include('../assets/includes/master.php');
?>
<html>
  <head>
    <noscript>
      <meta HTTP-EQUIV="REFRESH" content="0; url=https://eugenel.in/noscript">
    </noscript>
    <title>Register - <?php echo $site_title; ?></title>
    <link href="https://fonts.googleapis.com/icon?family=Material+Icons" rel="stylesheet">
    <link type="text/css" rel="stylesheet" href="../assets/css/materialize.css?<?php if ($reload_assets) { echo time(); } ?>"  media="screen,projection">
    <link type="text/css" rel="stylesheet" href="../assets/css/main.css?<?php if ($reload_assets) { echo time(); } ?>">
    <script src="https://code.jquery.com/jquery-2.1.4.min.js"></script>
    <script src="https://use.fontawesome.com/4857764df8.js"></script>
    <script src='https://www.google.com/recaptcha/api.js'></script>
    <script src="../assets/js/materialize.js?<?php if ($reload_assets) { echo time(); } ?>"></script>
    <script src="../assets/js/jquery.md5.js?<?php if ($reload_assets) { echo time(); } ?>"></script>
    <script>
      function callback() { // empty function for reCAPTCHA to callback
      }
      function register() {
        var captcha_data = grecaptcha.getResponse(); // get captcha response
        if ($('#password').val() == "" || $('#username').val() == "" || $('#password_confirm').val() == "") { // check no fields are empty
          Materialize.toast("Please complete all fields!"); // tell user to complete all fields
        } else {
          var pass = $.md5($('#password').val()); // get password hash
          var confirm = $.md5($('#password_confirm').val()); // same for confirmation
          var username = escape($('#username').val()); // escape string, in case someone does try injection
          if (pass == confirm) { // if the passwords match
            $.ajax({ // start ajax
              type: "POST", // post
              url: "../api/register.php", // to register endpoint
              data: "data={\"username\":\"" + escape($('#username').val()) + "\",\"md5\":\"" + pass + "\",\"captcha\":\"" + captcha_data + "\"}", // create json
              success: function(data) { // on success
                var obj = JSON.parse(data); // parse to object
                if (obj.status == 1) { // if success
                  Materialize.toast("Registration complete!"); // notify the user
                  var button = $('#register'); // get the actual element
                  button.addClass('disabled'); // add a class
                  button.html('Registered'); // change text
                } else {
                  Materialize.toast("Error: " + obj.content); // tell the user the error
                  grecaptcha.reset(); // reset
                }
              }
            });
          } else {
            Materialize.toast("Password and confirmation don't match!"); // notify user
          }
        }
      }
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
      });
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
        <h4>Register for an Account</h4>
        <div class="row">
          <form class="col s12">
            <div class="row">
              <div class="input-field col s12">
                <input id="username" type="text">
                <label for="username">Username (4-32 characters, only alphanumeric, dot, dash, and underscore are permitted</label>
              </div>
            </div>
            <div class="row">
              <div class="input-field col s12">
                <input id="password" type="password">
                <label for="password">Password</label>
              </div>
            </div>
            <div class="row">
              <div class="input-field col s12">
                <input id="password_confirm" type="password">
                <label for="password_confrim">Confirm Password</label>
              </div>
            </div>
            <div class="row">
              <center>
                <button id="register" class="g-recaptcha waves-effect waves-dark btn-large light-blue darken-2" data-sitekey="6LfV2lQUAAAAAJPi7mnk6Vr0RmM911ORKyw1xw0_" data-callback="register" style="min-width: 98%">Register</button>
              </center>
            </div>
          </form>
        </div>
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
