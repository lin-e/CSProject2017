<?php
  include('../assets/include/config.php');
?>
<html>
  <head>
    <noscript>
      <meta HTTP-EQUIV="REFRESH" content="0; url=https://eugenel.in/noscript">
    </noscript>
    <title>Lorem - <?php echo $site_title; ?></title>
    <link rel="shortcut icon" href="../assets/favicon.ico">
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
        <h4>Lorem ipsum</h4>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Lorem ipsum dolor sit amet, consectetur adipiscing elit. Maecenas facilisis, arcu quis scelerisque consectetur, odio mauris tincidunt est, tempus ultrices metus justo quis sapien. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer iaculis semper sem, nec viverra neque. Cras condimentum lectus magna, et fringilla nisi dignissim ac. Phasellus in lacus mi. Vivamus ornare iaculis ultrices. Quisque sodales purus nec arcu volutpat, at sagittis purus bibendum. Interdum et malesuada fames ac ante ipsum primis in faucibus.</p>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Vivamus varius, sapien quis viverra aliquet, magna nulla faucibus lectus, in molestie ligula tellus dapibus mauris. Nulla ac iaculis neque. Quisque metus libero, condimentum vel mi quis, condimentum porttitor sapien.</p>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Ut mattis luctus sollicitudin. Phasellus rutrum enim non ullamcorper rutrum. Integer at nulla nec magna porttitor bibendum. Nulla vehicula risus ultricies orci aliquam pharetra. Donec pulvinar consequat quam, molestie tincidunt mi euismod nec.</p>
        <div class="divider"></div>
        <h4>Sed ullamcorper</h4>
        <h5>Phasellus gravida</h5>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Phasellus ac consequat eros, ut aliquam ipsum. Maecenas et mi sapien. Duis metus augue, vestibulum nec tortor quis, commodo imperdiet nisi. Mauris et lorem nec ipsum gravida laoreet in ac nulla. Fusce pulvinar elementum eros quis aliquet. Aliquam egestas laoreet mi non auctor. Proin aliquam tellus non diam sodales, id vestibulum purus laoreet. Vivamus tempus tempor purus ut pellentesque. Aenean iaculis turpis eget eros lacinia imperdiet. Nam sem ligula, consectetur ac felis et, euismod convallis sem. Nulla accumsan augue at sodales vulputate. Duis pretium magna eget felis congue blandit. In nec lorem libero.</p>
        <blockquote>
          <span>&nbsp;&nbsp;&nbsp;&nbsp;Cras ultricies sapien et iaculis hendrerit. Aliquam imperdiet turpis eu sollicitudin fringilla. Nunc pellentesque ex at augue elementum consequat. Nulla mollis nisi orci, in eleifend mi scelerisque fringilla. Aenean pharetra metus ac justo porttitor, non sagittis ex cursus. Nunc facilisis neque ultrices nisi vestibulum, ac volutpat urna congue. Donec molestie sapien sed nibh mattis, eget egestas dolor mollis. Donec a purus tellus.</span>
        </blockquote>
        <blockquote>
          <span>&nbsp;&nbsp;&nbsp;&nbsp;Nulla porttitor consectetur magna, at ullamcorper nisl gravida quis. Fusce vitae convallis ex. Sed faucibus magna sed diam faucibus, et porta dolor porta. Fusce luctus molestie tortor sit amet gravida. Aliquam pharetra porta mi, at tincidunt nisl consequat quis. Maecenas ut augue maximus, fermentum ante vitae, imperdiet orci. Quisque pharetra in velit ac faucibus. Nulla interdum fringilla neque sed imperdiet. Duis egestas sodales varius.</span>
        </blockquote>
        <h5>Morbi non nulla</h5>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Morbi nec libero eget velit ullamcorper laoreet quis in enim. Fusce tincidunt elit tristique dui eleifend, ut pellentesque metus lacinia. Fusce non sem mattis, luctus justo convallis, bibendum quam. Cras eget consectetur velit. Vivamus blandit, nulla eu hendrerit porta, massa ipsum laoreet orci, eu rhoncus risus quam molestie ex. Nullam congue laoreet dolor, sed vehicula mi finibus vel.</p>
        <div class="divider"></div>
        <h4>Fusce at rhoncus purus</h4>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Aenean nec nibh eget enim rutrum aliquam. Aliquam maximus turpis sed purus malesuada, ac faucibus felis sodales. Vivamus est turpis, semper eget mauris non, auctor mollis dui. Aliquam mollis, metus ut vestibulum rhoncus, metus justo mollis felis, at lacinia enim enim a elit. Morbi vitae risus semper, placerat quam ut, ornare turpis. Phasellus rhoncus convallis fringilla. In massa est, dapibus sit amet massa vel, cursus varius magna. In non convallis purus, vitae lacinia justo. Vestibulum purus nisi, porttitor ut posuere a, pharetra sit amet lorem. Fusce hendrerit at ligula eu ornare.</p>
        <h5>Praesent magna quam</h5>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Aenean consectetur interdum lacus scelerisque varius. Nullam quis auctor mi. Phasellus non turpis a leo tristique varius vitae vitae erat. Maecenas euismod semper enim eu ullamcorper. In auctor, tortor nec hendrerit convallis, elit eros pellentesque augue, nec pretium neque ligula ac sapien. Phasellus ultrices, nulla lobortis volutpat cursus, nibh risus imperdiet velit, non consequat sem turpis et massa.</p>
        <h5>Donec venenatis</h5>
        <p>&nbsp;&nbsp;&nbsp;&nbsp;Phasellus eu velit augue. Nunc dignissim vehicula magna, vitae dictum velit luctus at. Morbi accumsan nulla arcu, et iaculis leo porttitor nec. Proin magna mauris, tincidunt eget pharetra sit amet, pretium ac sem. Donec et fringilla purus, a elementum lorem. Donec luctus, quam eget iaculis rhoncus, diam mauris accumsan libero, sed ultrices purus ante ac nibh. Ut non vestibulum eros, at lobortis libero. Donec eget ante erat. Integer leo est, porttitor et arcu quis, ullamcorper lacinia ante. Praesent aliquet gravida mauris, ut facilisis felis vestibulum a. Class aptent taciti sociosqu ad litora torquent per conubia nostra, per inceptos himenaeos. Nam est magna, sollicitudin sit amet elit in, congue tempor lacus.</p>
        <div class="divider"></div>
        <blockquote>
          <span>This is a page to test the layout of Materialize. The text here has been generated by <a class="grey-text" href="http://www.lipsum.com/">lipsum.com</a>.</span>
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
