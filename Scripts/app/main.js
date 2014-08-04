/*-- Global variables --*/
var map, mc, tweetLayer, trendLayer, counter = 0, bounds = new google.maps.LatLngBounds(), twitterHub = $.connection.geoFeedHub, startBtn = $('#startStream'), stopBtn = $('#stopStream'), iconBase = 'http://dev.wherelionsroam.co.uk/';
var stylez = [{
    "featureType": "road",
    "stylers": [
        {
            "visibility": "off"
        }
    ]
},
    {
        "featureType": "transit",
        "stylers": [
            {
                "visibility": "off"
            }
        ]
    }, { "featureType": "water", "elementType": "geometry", "stylers": [{ "visibility": "on" }, { "color": "#C6E2FF" }] }, { "featureType": "poi", "elementType": "geometry.fill", "stylers": [{ "color": "#C5E3BF" }] }, { "featureType": "road", "elementType": "geometry.fill", "stylers": [{ "color": "#D1D1B8" }] }];
var map_options = {
    backgroundColor: '#A3A3A3',
    disableDoubleClickZoom: true,
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
var buildTrendWindow = function(trend){
    var tweets = trend.tweets;
    var html = "<div class='trendTweetsWrapper'><h3>Tweets for " + trend.title + "</h3><ul>";
     //console.log(trend);
     $.each(trend.data.tweets,function(i,item){
         
         html += "<li><div class='tweetbody'>" + item.Text + "</div><div class='tweetfoot'><span class='tweetlink'><a target='_blank' href='" + item.URL + "'>View on Twitter</a></span></div></li>";
    });
    html += "</ul></div>";
    return html;
};
var buildTweetWindow = function(tweet) {
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
function fitToUKBounds(map) {
    var topLeft = new google.maps.LatLng(49.955269, -8.164723);
    var bottomRight = new google.maps.LatLng(60.6311, 1.7425);
    var bounds = new google.maps.LatLngBounds(topLeft, bottomRight);
    map.fitBounds(bounds);
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
//function killStream() {
//    twitterHub.server.killStream().done(function () {
//        alert('Stream is dead. This will cause exception in app');
//    }).fail(function (e) {
//        alert(e);
//    });
//}
function Initialize() {

    // geocomplete
    $('#autocomplete').geocomplete({
        map: "#map-canvas",
        mapOptions: map_options,
        markerOptions:{
          disabled:true
        }
    }).bind("geocode:result", function (e, result) {

        var resultBounds = new google.maps.LatLngBounds(
            result.geometry.viewport.getSouthWest(),
            result.geometry.viewport.getNorthEast()
        );
        map.fitBounds(resultBounds);
        // create new notification
        var notification = new NotificationFx({
            message: '<span class="icon"><i class="fa fa-map-marker fa-3x"></i></span><p>Now only showing results for <strong>' + result.name + '</strong> and surrounding areas.</p>',
            layout: 'bar',
            customClass: 'geo',
            effect: 'exploader',
            ttl: 10000,
            type: 'notice'
        });

        // show the notification
        notification.show();
    });
    $('#find').click(function () {
        $("#autocomplete").trigger("geocode");
    });
    map = $("#autocomplete").geocomplete("map");
    fitToUKBounds(map);
    tweetLayer = new CustomOverlay(map,false);
    trendLayer = new CustomOverlay(map,true);
    map.set('streetViewControl', true);
    map.mapTypeControl = false;
    var clusterer_opts = {
        gridSize: 100,
        batchSize: 3000,
        minimumClusterSize:1,
        batchSizeIE: 200,
        maxZoom: 12,
        averageCenter: false,
        ignoreHidden:true
    };

    mc = new MarkerClusterer(map, [], clusterer_opts);
}

/* Doc Ready stuff */
$(function () {

    Initialize();

            twitterHub.client.BroadcastNewUserToHub = function (user) {
                console.log(user);
            };
           twitterHub.client.broadcastTweetMessage = function (tweet) {
               counter++;
               $('#tweet-count').html(counter);
               // add marker to the map
               var marker = new google.maps.Marker({
                   position: new google.maps.LatLng(tweet.Latitude, tweet.Longitude),
                   title: tweet.Text + ' by @' + tweet.User
               });
               if(tweetLayer.hidden){
                  marker.setVisible(false);
               }
               tweetLayer.addOverlay(marker);
               //tweetLayer.setMap(map);
               var infowindow = new google.maps.InfoWindow({
                   content:buildTweetWindow(tweet)
               });
               $('time.timeago').timeago();
               google.maps.event.addListener(marker, 'click', function () {
                   map.panTo(marker.getPosition());
                   infowindow.open(map, marker);
               });
               mc.addMarker(marker);
               mc.repaint();
               // mc.repaint();
               // add to console, and clear out earliest list item if list size > arbitrary number
               var list_size = $('ul.live-tweets li').size();
               var max_list_size = 50;
               var list_item = '<li class="tweet-item"><span class="tweet-avatar"><a target="_blank" href="https://www.twitter.com/' + tweet.User + '"><img alt="' + tweet.User + 's profile picture" class="img-thumbnail" src="' + tweet.ImageUrl + '"/></a></span><span class="tweet-content">' + tweet.Text + '</span><span class="tweet-author">@' + tweet.User + '</span></li>';

               // randomize selection of tweets for display to ensure that tweets can be read.

               if (list_size < max_list_size) {
                   $('ul.live-tweets').prepend(list_item);
               } else {
                   $('ul.live-tweets li:last-child').remove();
                   $('ul.live-tweets').prepend(list_item);
               }
           }
           twitterHub.client.broadcastLog = function (message) {
               console.log(message);
           };

           $('#switch').click(function () {
               $('.loader').show('fade', 100);
               setTimeout(function () {
                   // if trend layer isHidden = true
                   if (trendLayer.getState()) {
                       //mc.setClusterClass('hide');
                       mc.setMap(null);
                       tweetLayer.hide();
                       trendLayer.show();
                       $('#switch').removeClass('btn-warning');
                       $('#switch').addClass('btn-default');
                       $('#switch').html('Switch to Tweets');
                   } else {
                       mc.repaint();
                       mc.setMap(map);
                   
                       trendLayer.hide();
                       tweetLayer.show();
                       $('#switch').addClass('btn-warning');
                       $('#switch').removeClass('btn-default');
                       $('#switch').html('Switch to Trends');
                   }
                   $('.loader').hide('fade', 100);
               }, 600);
           });
           $('#reset-map').click(function () {
               fitToUKBounds(map);
           });
           $.connection.hub.start()
               .done(function () {
                   subscribeGlobalStream();
                   //killBtn.click(function (e) {
                   //    e.preventDefault();
                   //    killStream();
                   //});
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

           twitterHub.client.broadcastTrend = function (entity) {
               var marker = new google.maps.Marker({
                   position: new google.maps.LatLng(entity.averageCenter.Longitude, entity.averageCenter.Latitude),
                   title: entity.Name,
                   data: {
                       tweets: entity.tweets,
                       type: entity.entityType,
                       miscInfo: entity.MiscInformation
                   },
                   icon: '/img/trends/' + entity.entityType + '.png'
               });
               console.log(marker.data);
               var infowindow = new google.maps.InfoWindow({
                   content: buildTrendWindow(marker),
                   maxWidth: 400,
                   pixelOffset:new google.maps.Size(15,15)
               });
               google.maps.event.addListener(marker, 'click', function () {
                   map.panTo(marker.getPosition());
                   infowindow.open(map, marker);
               });
               trendLayer.addOverlay(marker);
           };
            
           google.maps.event.addListener(map, 'idle', function (event) {
               google.maps.event.addListener(map, 'bounds_changed', function () {
                   var bounds = this.getBounds();
                   var sw = bounds.getSouthWest();
                   var ne = bounds.getNorthEast();
                   var nw = new google.maps.LatLng(ne.lat(), sw.lng());
                   var se = new google.maps.LatLng(sw.lat(), ne.lng());

                   twitterHub.server.changeStreamBounds({ SouthEastLongitude: se.lng(), SouthEastLatitude: se.lat(), NorthWestLongitude: nw.lng(), NorthWestLatitude: nw.lat() }, $.connection.hub.id).done(function () {
                       console.log('Bounds changed');
                   }).fail(function (error) {
                       console.log('Bounds change failure. Error: ' + error);
                   });
               });
           });
       });
