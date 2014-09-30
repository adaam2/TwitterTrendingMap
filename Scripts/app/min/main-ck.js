function fitToUKBounds(e){var t=new google.maps.LatLng(49.955269,-8.164723),a=new google.maps.LatLng(60.6311,1.7425),o=new google.maps.LatLngBounds(t,a);e.fitBounds(o)}function subscribeGlobalStream(){twitterHub.server.subscribeToStreamGroup("Global").done(function(e){e===!0&&(startBtn.prop("disabled",!0),stopBtn.prop("disabled",!1))})}function unsubscribeGlobalStream(){twitterHub.server.unsubscribeFromStreamGroup("Global").done(function(){stopBtn.prop("disabled",!0),startBtn.prop("disabled",!1)})}function Initialize(){$("#autocomplete").geocomplete({map:"#map-canvas",mapOptions:map_options,markerOptions:{disabled:!0}}).bind("geocode:result",function(e,t){var a=new google.maps.LatLngBounds(t.geometry.viewport.getSouthWest(),t.geometry.viewport.getNorthEast());map.fitBounds(a);var o=new NotificationFx({message:'<span class="icon"><i class="fa fa-map-marker fa-3x"></i></span><p>Now only showing results for <strong>'+t.name+"</strong> and surrounding areas. (The number of tweets broadcasted should now be far less.)</p>",layout:"bar",customClass:"geo",effect:"exploader",ttl:1e4,type:"notice"});o.show()}),$("#find").click(function(){$("#autocomplete").trigger("geocode")}),map=$("#autocomplete").geocomplete("map"),fitToUKBounds(map),tweetLayer=new CustomOverlay(map,!1),trendLayer=new CustomOverlay(map,!0),map.set("streetViewControl",!0),map.mapTypeControl=!1;var e={gridSize:100,batchSize:3e3,minimumClusterSize:2,batchSizeIE:500,maxZoom:10,averageCenter:!1,ignoreHidden:!0};mc=new MarkerClusterer(map,[],e)}var map,mc,addedToMap=[],tweetLayer,trendLayer,counter=0,bounds=new google.maps.LatLngBounds,twitterHub=$.connection.geoFeedHub,startBtn=$("#startStream"),stopBtn=$("#stopStream"),iconBase="http://dev.wherelionsroam.co.uk/",stylez=[{featureType:"water",stylers:[{saturation:43},{lightness:-11},{hue:"#0088ff"}]},{featureType:"road",elementType:"geometry.fill",stylers:[{hue:"#ff0000"},{saturation:-100},{lightness:99}]},{featureType:"road",elementType:"geometry.stroke",stylers:[{color:"#808080"},{lightness:54}]},{featureType:"landscape.man_made",elementType:"geometry.fill",stylers:[{color:"#ece2d9"}]},{featureType:"poi.park",elementType:"geometry.fill",stylers:[{color:"#ccdca1"}]},{featureType:"road",elementType:"labels.text.fill",stylers:[{color:"#767676"}]},{featureType:"road",elementType:"labels.text.stroke",stylers:[{color:"#ffffff"}]},{featureType:"poi",stylers:[{visibility:"off"}]},{featureType:"landscape.natural",elementType:"geometry.fill",stylers:[{visibility:"on"},{color:"#b8cb93"}]},{featureType:"poi.park",stylers:[{visibility:"on"}]},{featureType:"poi.sports_complex",stylers:[{visibility:"on"}]},{featureType:"poi.medical",stylers:[{visibility:"on"}]},{featureType:"poi.business",stylers:[{visibility:"simplified"}]}],map_options={backgroundColor:"#A3A3A3",disableDoubleClickZoom:!0,mapTypeID:google.maps.MapTypeId.HYBRID,keyboardShortcuts:!1,overviewMapControl:!1,mapTypeControlOptions:{mapTypeIds:[google.maps.MapTypeId.ROADMAP]},panControl:!1,scrollwheel:!1,streetViewControl:!1,styles:stylez},buildTrendWindow=function(e){var t=e.tweets,a="<div class='trendTweetsWrapper'><h3>Tweets for "+e.title+"</h3><ul>";return $.each(e.data.tweets,function(e,t){a+="<li class='cf'><div class='tweetmain'><div class='tweetbody'>"+t.Text+"</div><div class='tweetfoot'><span class='tweetlink'><a target='_blank' href='"+t.URL+"'>View on Twitter</a></span></div></div><div class='tweetavatar'><img src='"+t.ImageUrl+"'/></div></li>"}),a+="</ul></div>"},buildTweetWindow=function(e){var t=new Date(e.CreatedAt),a=t.getDate(),o=t.getMonth()+1,s=t.getFullYear(),n=t.toTimeString(),i=a+"/"+o+"/"+s,r='<div class="tweet">        <div class="header">        <div class="avatar"><a target="_blank" href="https://www.twitter.com/'+e.User+'"><img src="'+e.ImageUrl+'" alt="'+e.User+'s profile picture"/></a></div>        <div class="screen-name"><span class="handle">@'+e.User+'</span><span class="time-ago"><time class="timeago" datetime="'+e.CreatedAt+'">'+i+'</time></span></div>        </div>        <div class="body"><p>'+e.Text+'</p></div>        <div class="footer">'+i+" "+n+"</div>        </div>";return r};$(function(){$(".entity-tweets-link").fancybox({helpers:{overlay:{locked:!1}}}),Initialize(),twitterHub.client.broadcastTweetMessage=function(e){counter++,$("#tweet-count").html(counter);var t=new google.maps.Marker({position:new google.maps.LatLng(e.Latitude,e.Longitude),title:e.Text+" by @"+e.User});tweetLayer.hidden&&t.setVisible(!1),tweetLayer.addOverlay(t);var a=new google.maps.InfoWindow({content:buildTweetWindow(e)});$("time.timeago").timeago(),google.maps.event.addListener(t,"click",function(){map.panTo(t.getPosition()),a.open(map,t)}),mc.addMarker(t),mc.repaint();var o=$("ul.live-tweets li").size(),s=50,n='<li class="tweet-item"><span class="tweet-avatar"><a target="_blank" href="https://www.twitter.com/'+e.User+'"><img alt="'+e.User+'s profile picture" class="img-thumbnail" src="'+e.ImageUrl+'"/></a></span><span class="tweet-content">'+e.Text+'</span><span class="tweet-author">@'+e.User+"</span></li>";s>o?$("ul.live-tweets").prepend(n):($("ul.live-tweets li:last-child").remove(),$("ul.live-tweets").prepend(n))},$("#switch").click(function(){$(".loader").show("fade",100),setTimeout(function(){trendLayer.getState()?(mc.repaint(),mc.setMap(null),tweetLayer.hide(),trendLayer.show(),$(".console h3.title").text("Live Trends"),$(".live-tweets").addClass("hidden"),$(".live-trends").removeClass("hidden"),$("#switch").removeClass("btn-warning"),$("#switch").addClass("btn-default"),$("#switch").html("Switch to Tweets")):(mc.repaint(),mc.setMap(map),trendLayer.hide(),tweetLayer.show(),$(".live-trends").addClass("hidden"),$(".live-tweets").removeClass("hidden"),$(".console h3.title").text("Live Tweets"),$("#switch").addClass("btn-warning"),$("#switch").removeClass("btn-default"),$("#switch").html("Switch to Trends")),$(".loader").hide("fade",100)},300)}),$("#reset-map").click(function(){fitToUKBounds(map)}),$.connection.hub.start().done(function(){subscribeGlobalStream(),stopBtn.click(function(e){unsubscribeGlobalStream(),e.preventDefault();var t=new NotificationFx({message:'<span class="icon icon-settings"></span><p>The stream has been stopped. You will not receive any tweets on the map, nor will you be able to see any incoming trends until you restart the stream.</p>',layout:"bar",customClass:"stop",effect:"exploader",ttl:5e3,type:"notice"});t.show()}),startBtn.click(function(e){subscribeGlobalStream(),e.preventDefault();var t=new NotificationFx({message:'<span class="icon icon-settings"></span><p>You have started the stream. Incoming tweets and trends will match the current viewport of the map.</p>',layout:"bar",customClass:"start",effect:"exploader",ttl:5e3,type:"notice"});t.show()})}),twitterHub.client.broadcastTrend=function(e){var t='<li class="trend '+e.entityType.toLowerCase()+'">'+e.Name+'<span class="plusone">'+e.tweets.length+" Mentions (+1)</span></li>";if($(".live-trends").prepend(t),e.tweets.length>=5&&-1==$.inArray(e.Name,addedToMap)){addedToMap.push(e.Name);var a=new google.maps.Marker({position:new google.maps.LatLng(e.averageCenter.Longitude,e.averageCenter.Latitude),title:e.Name,data:{tweets:e.tweets,type:e.entityType},icon:"/img/trends/"+e.entityType+".png"}),o=new google.maps.InfoWindow({content:buildTrendWindow(a),maxWidth:400,pixelOffset:new google.maps.Size(15,15)});google.maps.event.addListener(a,"click",function(){map.panTo(a.getPosition()),o.open(map,a)}),trendLayer.addOverlay(a)}},$("#localSearch").geocomplete().bind("geocode:result",function(e,t){var a=new google.maps.LatLngBounds(t.geometry.viewport.getSouthWest(),t.geometry.viewport.getNorthEast()),o=a.getSouthWest(),s=a.getNorthEast(),n=new google.maps.LatLng(s.lat(),o.lng()),i=new google.maps.LatLng(o.lat(),s.lng());twitterHub.server.GetTopEntitiesGeo({SouthEastLongitude:i.lng(),SouthEastLatitude:i.lat(),NorthWestLongitude:n.lng(),NorthWestLatitude:n.lat()}).done(function(e){console.log(e)}).fail(function(e){console.log("Error: "+e)})}),google.maps.event.addListener(map,"idle",function(e){google.maps.event.addListener(map,"bounds_changed",function(){var e=this.getBounds(),t=e.getSouthWest(),a=e.getNorthEast(),o=new google.maps.LatLng(a.lat(),t.lng()),s=new google.maps.LatLng(t.lat(),a.lng());twitterHub.server.changeStreamBounds({SouthEastLongitude:s.lng(),SouthEastLatitude:s.lat(),NorthWestLongitude:o.lng(),NorthWestLatitude:o.lat()},$.connection.hub.id).done(function(){console.log("Bounds changed")}).fail(function(e){console.log("Bounds change failure. Error: "+e)})})})});