var map, mc;
var clusters = [
    {
        textColor: 'white',
        url: 'path/to/smallclusterimage.png',
        height: 50,
        width: 50
    },
    {
        textColor: 'white',
        url: 'path/to/smallclusterimage.png',
        height: 50,
        width: 50
    },
    {
        textColor: 'white',
        url: 'path/to/smallclusterimage.png',
        height: 50,
        width: 50
    }
];
$(function () {
    function initialize() {
        var stylez = [{ "featureType": "water", "stylers": [{ "saturation": 43 }, { "lightness": -11 }, { "hue": "#0088ff" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "hue": "#ff0000" }, { "saturation": -100 }, { "lightness": 99 }] }, { "featureType": "road", "elementType": "geometry.stroke", "stylers": [{ "color": "#808080" }, { "lightness": 54 }] }, { "featureType": "landscape.man_made", "elementType": "geometry.fill", "stylers": [{ "color": "#ece2d9" }] }, { "featureType": "poi.park", "elementType": "geometry.fill", "stylers": [{ "color": "#ccdca1" }] }, { "featureType": "road", "elementType": "labels.text.fill", "stylers": [{ "color": "#767676" }] }, { "featureType": "road", "elementType": "labels.text.stroke", "stylers": [{ "color": "#ffffff" }] }, { "featureType": "poi", "stylers": [{ "visibility": "off" }] }, { "featureType": "landscape.natural", "elementType": "geometry.fill", "stylers": [{ "visibility": "on" }, { "color": "#b8cb93" }] }, { "featureType": "poi.park", "stylers": [{ "visibility": "on" }] }, { "featureType": "poi.sports_complex", "stylers": [{ "visibility": "on" }] }, { "featureType": "poi.medical", "stylers": [{ "visibility": "on" }] }, { "featureType": "poi.business", "stylers": [{ "visibility": "simplified" }] }];
        var map_options = {
            zoom: 5,
            backgroundColor: '#A3A3A3',
            disableDoubleClickZoom: true,
            center: new google.maps.LatLng(53, -0.2),
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
        var date = new Date(tweet.CreatedAt);
        var dd = date.getDate();
        var mm = date.getMonth() + 1;
        var yy = date.getFullYear();
        var minutes = date.toTimeString();
        var formatted_date = dd + '/' + mm + '/' + yy;
        //var timeago = jQuery.timeago(tweet.CreatedAt);
        var content = '<div class="tweet">\
        <div class="header">\
        <div class="avatar"><a target="_blank" href="https://www.twitter.com/' + tweet.User + '"><img src="' + tweet.ImageUrl + '"/></a></div>\
        <div class="screen-name"><span class="handle">@' + tweet.User + '</span><span class="time-ago"><time class="timeago" datetime="' + tweet.CreatedAt + '">' + tweet.CreatedAt + '</time></span></div>\
        </div>\
        <div class="body"><p>' + tweet.Text + '</p></div>\
        <div class="footer">' + formatted_date + ' ' + minutes + '</div>\
        </div>';
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
    // geocomplete
    $('#autocomplete').geocomplete().bind("geocode:result", function (e, result) {
        map.setCenter(result.geometry.location);
        map.setZoom(13);
    });
           var twitterHub = $.connection.geoFeedHub,
               map;

           twitterHub.client.broadcastTweetMessage = function (tweet) {

               // add marker to the map
               var marker = new google.maps.Marker({
                   map:map,
                   position: new google.maps.LatLng(tweet.Latitude, tweet.Longitude),
                   title:tweet.Text + ' by @' + tweet.User
               });
               var infowindow = new google.maps.InfoWindow({
                   content:buildTweetWindow(tweet)
               });
               $('time.timeago').timeago();
               google.maps.event.addListener(marker, 'click', function () {
                   map.panTo(marker.getPosition());
                   infowindow.open(map, marker);
               });
               mc.addMarker(marker);

               // add to console, and clear out earliest list item if list size > arbitrary number
               var list_size = $('ul.live-tweets li').size();
               var max_list_size = 10;
               var list_item = '<li class="tweet-item"><span class="tweet-content">' + tweet.Text + '</span><span class="tweet-author">' + tweet.User + '</span></li>';
               if (list_size < max_list_size) {
                   $('ul.live-tweets').prepend(list_item);
               } else {
                   $('ul.live-tweets li:last-child').remove();
                   $('ul.live-tweets').prepend(list_item);
               }
           }
           twitterHub.client.broadcastStatus = function (status) {
               console.log(status.Message + '<br/>' + status.StackTrace);
           };
           $.connection.hub.start()
               .done(function () {
                   $('#signalr-status').text('Now connected (connection ID ' + $.connection.hub.id + ')');
               });
       });
