var map, mc;
$(function () {
    function initialize() {
        var stylez = [{ "featureType": "landscape", "stylers": [{ "saturation": -100 }, { "lightness": 65 }, { "visibility": "on" }] }, { "featureType": "poi", "stylers": [{ "saturation": -100 }, { "lightness": 51 }, { "visibility": "simplified" }] }, { "featureType": "road.highway", "stylers": [{ "saturation": -100 }, { "visibility": "simplified" }] }, { "featureType": "road.arterial", "stylers": [{ "saturation": -100 }, { "lightness": 30 }, { "visibility": "on" }] }, { "featureType": "road.local", "stylers": [{ "saturation": -100 }, { "lightness": 40 }, { "visibility": "on" }] }, { "featureType": "transit", "stylers": [{ "saturation": -100 }, { "visibility": "simplified" }] }, { "featureType": "administrative.province", "stylers": [{ "visibility": "off" }] }, { "featureType": "water", "elementType": "labels", "stylers": [{ "visibility": "on" }, { "lightness": -25 }, { "saturation": -100 }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "hue": "#8899a6" }, { "lightness": -25 }, { "saturation": -100 }] }];
        var map_options = {
            zoom: 3,
            backgroundColor: '#A3A3A3',
            disableDoubleClickZoom: true,
            center: new google.maps.LatLng(51, -0.2),
            mapTypeID: google.maps.MapTypeId.HYBRID,
            keyboardShortcuts: false,
            overviewMapControl: false,
            panControl: false,
            scrollwheel: false,
            streetViewControl: false,
            styles: stylez
        };
        map = new google.maps.Map(document.getElementById('map-canvas'), map_options);
        var clusterer_opts = {
            gridSize: 100,
            batchSize: 3000,
            batchSizeIE: 200,
            maxZoom: 12,
            averageCenter:false
        };
        mc = new MarkerClusterer(map, [], clusterer_opts);
    }
    function buildTweetWindow(tweet) {
        var content = '<div class="tweet"><img src="' + tweet.ImageUrl + '"/><div class="tweet-body">' + tweet.Text + '</div></div>';
        return content;
    }
    $('#minimize').click(function () {
        var toggle_div = $('.toggle');
        toggle_div.slideToggle();
        $(this).text(function (i, text) {
            return text === "More" ? "Less" : "More";
        })
    });
    initialize();

           var twitterHub = $.connection.geoFeedHub,
               map;
           twitterHub.client.rateLimited = function (message) {
               console.log(message);
           };
           twitterHub.client.broadcastTweetMessage = function (tweet) {
               console.log(tweet);
               var marker = new google.maps.Marker({
                   map:map,
                   position: new google.maps.LatLng(tweet.Latitude, tweet.Longitude),
                   title:tweet.Text + ' by @' + tweet.User
               });
               var infowindow = new google.maps.InfoWindow({
                   content:buildTweetWindow(tweet)
               });
               google.maps.event.addListener(marker, 'click', function () {
                   infowindow.open(map, marker);
               });
               mc.addMarker(marker);
               //console.log(marker);
           }

           $.connection.hub.start()
               .done(function () {
                   console.log('Now connected, connection ID = ' + $.connection.hub.id);
               });

       });
