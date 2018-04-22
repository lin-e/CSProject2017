<?php
  include('../assets/includes/config.php');
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
    <script src="../assets/js/materialize.js?<?php if ($reload_assets) { echo time(); } ?>"></script>
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
  $raw_sites = file_get_contents('../assets/json/navbar.json');
  $decoded_sites = json_decode($raw_sites);
  echo "        ".$decoded_sites->logo."\n";
  $normal_links = array();
  $mobile_links = array();
  foreach ($decoded_sites->navbar as $single_site) {
    if ($single_site->hide) {
      continue;
    }
    $url = $single_site->link;
    $fa = $single_site->fa;
    $name = $single_site->name;
    $normal_active = "";
    $mobile_active = "";
    if ($single_site->cwd == getcwd()) {
      $url = "#!";
      $normal_active = " active";
      $mobile_active = " class=\"active\"";
    }
    array_push($normal_links, "<li class=\"waves-effect waves-lighten$normal_active\"><a href=\"$url\"><i class=\"fa fa-fw fa-$fa\"></i>&nbsp; $name</a></li>");
    array_push($mobile_links, "<li$mobile_active><a href=\"$url\">$name</a></li>");
  }
?>
        <a href="#" data-activates="mobile-nav" class="button-collapse right"><i class="material-icons">menu</i></a>
        <ul class="right hide-on-med-and-down">
<?php
  $normal_padding = "          ";
  foreach ($normal_links as $normal_link) {
    echo $normal_padding.$normal_link."\n";
  }
?>
        </ul>
        <ul class="side-nav" id="mobile-nav">
<?php
  $mobile_padding = "          ";
  foreach ($mobile_links as $mobile_link) {
    echo $mobile_padding.$mobile_link."\n";
  }
?>
        </ul>
      </div>
    </nav>
    <main class="white-text">

    <p class="range-field red">
      <input type="range" class="red" id="test5" min="0" max="255" />
    </p>
      <div class="container">
        <h4>Register for an Account</h4>
        <div class="divider"></div>
        <blockquote>
          <span>To be implemented</span>
        </blockquote>
      </div>
    </main>
    <footer class="page-footer">
<?php
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
