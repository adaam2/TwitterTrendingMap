var map, mc, guids = [], counter = 0, bounds = new google.maps.LatLngBounds(), twitterHub = $.connection.geoFeedHub, startBtn = $('#startStream'), stopBtn = $('#stopStream'),svgshape = document.getElementById('notification-shape');;
var stylez = [{ "featureType": "road", "elementType": "geometry", "stylers": [{ "lightness": 100 }, { "visibility": "simplified" }] }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#C6E2FF" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#C5E3BF" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "color": "#D1D1B8" }] }];
var map_options = {
    zoom: 7,
    backgroundColor: '#A3A3A3',
    disableDoubleClickZoom: true,
    center: new google.maps.LatLng(53, -0.2),
    mapTypeID: google.maps.MapTypeId.HYBRID,
    keyboardShortcuts: false,
    overviewMapControl: false,
    mapTypeControlOptions: {
        mapTypeIds: [google.maps.MapTypeId.ROADMAP]
    },
    panControl: false,
    scrollwheel: false,
    streetViewControl: false,
    styles: stylez
};
var guid = (function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
                   .toString(16)
                   .substring(1);
    }
    return function () {
        return s4() + s4();
    };
})();

function buildTweetWindow(tweet) {
    var date = new Date(tweet.CreatedAt);
    var dd = date.getDate();
    var mm = date.getMonth() + 1;
    var yy = date.getFullYear();
    var minutes = date.toTimeString();
    var formatted_date = dd + '/' + mm + '/' + yy;
    var content = '<div class="tweet">\
        <div class="header">\
        <div class="avatar"><a target="_blank" href="https://www.twitter.com/' + tweet.User + '"><img src="' + tweet.ImageUrl + '" alt="' + tweet.User + 's profile picture"/></a></div>\
        <div class="screen-name"><span class="handle">@' + tweet.User + '</span><span class="time-ago"><time class="timeago" datetime="' + tweet.CreatedAt + '">' + formatted_date + '</time></span></div>\
        </div>\
        <div class="body"><p>' + tweet.Text + '</p></div>\
        <div class="footer">' + formatted_date + ' ' + minutes + '</div>\
        </div>';
    return content;
}
function subscribeGlobalStream() {

    twitterHub.server.subscribeToStreamGroup("Global").done(function (result) {

        if (result === true) {

            startBtn.prop("disabled", true);
            stopBtn.prop("disabled", false);
        }
    });
}

function unsubscribeGlobalStream() {

    twitterHub.server.unsubscribeFromStreamGroup("Global").done(function () {
        stopBtn.prop("disabled", true);
        startBtn.prop("disabled", false);
    });
}
/* Doc Ready stuff */
$(function () {

    // geocomplete
    $('#autocomplete').geocomplete({
        map: "#map-canvas",
        mapOptions: map_options
    }).bind("geocode:result", function (e, result) {
        var resultBounds = new google.maps.LatLngBounds(
            result.geometry.viewport.getSouthWest(),
            result.geometry.viewport.getNorthEast()
        );
        map.fitBounds(resultBounds);
    });
    $('#find').click(function () {
        $("#autocomplete").trigger("geocode");
    });
    var map = $("#autocomplete").geocomplete("map");
    map.set('streetViewControl', true);
    map.mapTypeControl = false;
    var clusterer_opts = {
        gridSize: 100,
        batchSize: 3000,
        batchSizeIE: 200,
        maxZoom: 12,
        averageCenter: false
    };

    mc = new MarkerClusterer(map, [], clusterer_opts);

            twitterHub.client.BroadcastNewUserToHub = function (user) {
                console.log(user);
            };
           twitterHub.client.broadcastTweetMessage = function (tweet) {
               counter++;
               $('#tweet-count').html(counter);
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
               var list_item = '<li class="tweet-item"><span class="tweet-avatar"><a target="_blank" href="https://www.twitter.com/' + tweet.User + '"><img alt="' + tweet.User + 's profile picture" class="img-thumbnail" src="' + tweet.ImageUrl + '"/></a></span><span class="tweet-content">' + tweet.Text + '</span><span class="tweet-author">@' + tweet.User + '</span></li>';


               // randomize selection of tweets for display to ensure that tweets can be read.

               if (list_size < max_list_size) {
                   $('ul.live-tweets').prepend(list_item);
               } else {
                   $('ul.live-tweets li:last-child').remove();
                   $('ul.live-tweets').prepend(list_item);
               }
           }
           twitterHub.client.debug = function (msg) {
               console.log(msg);
           }
           twitterHub.client.broadcastLog = function (message) {
               $('#error').modal();
               console.log(message);
           };
           $.connection.hub.start()
               .done(function () {
                   subscribeGlobalStream();

                   stopBtn.click(function (e) {

                       unsubscribeGlobalStream();
                       e.preventDefault();

                       var notification = new NotificationFx({
                           message: '<span class="icon icon-settings"></span><p>The stream has been stopped. You will not receive any tweets on the map, nor will you be able to see any incoming trends until you restart the stream.</p>',
                           layout: 'bar',
                           customClass:'stop',
                           effect: 'exploader',
                           ttl: 6000,
                           type: 'notice'
                       });

                       // show the notification
                       notification.show();
                   });

                   startBtn.click(function (e) {

                       subscribeGlobalStream();
                       e.preventDefault();
                       // create the notification
                       var notification = new NotificationFx({
                           message: '<span class="icon icon-settings"></span><p>You have started the stream. Incoming tweets and trends will match the current viewport of the map.</p>',
                           layout: 'bar',
                           customClass:'start',
                           effect: 'exploader',
                           ttl: 6000,
                           type: 'notice'
                       });

                       // show the notification
                       notification.show();
                   });
               });

           var trendsAnalysisHub = $.connection.trendsAnalysisHub;
           trendsAnalysisHub.client.broadcastTrend = function (entity) {
               console.log(entity);
           };
           google.maps.event.addListener(map, 'idle', function (event) {
               google.maps.event.addListener(map, 'bounds_changed', function () {
                   var bounds = this.getBounds();
                   var sw = bounds.getSouthWest();
                   var ne = bounds.getNorthEast();
                   var nw = new google.maps.LatLng(ne.lat(), sw.lng());
                   var se = new google.maps.LatLng(sw.lat(), ne.lng());

                   twitterHub.server.changeStreamBounds({ SouthEastLongitude: se.lng(), SouthEastLatitude: se.lat(), NorthWestLongitude: nw.lng(), NorthWestLatitude: nw.lat() }).done(function () {
                       console.log('Bounds changed');
                   }).fail(function (error) {
                       console.log('Bounds change failure. Error: ' + error);
                   });
               });
           });
       });
