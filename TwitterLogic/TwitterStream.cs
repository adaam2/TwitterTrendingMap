using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading.Tasks;
using Tweetinvi.Streams;
using Tweetinvi.Credentials;
using Tweetinvi.Core.Interfaces.Models;
using Tweetinvi.Core.Interfaces.Streaminvi;
using System.Threading;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using FinalUniProject.Hubs;
using Newtonsoft.Json;
using Tweetinvi;
using FinalUniProject.Models;
using Tweetinvi.Logic.Model;
using Tweetinvi.Core.Enum;

namespace FinalUniProject.TwitterLogic
{
    public class TwitterStream
    {
        // Twitter Access Keys
        private string _consumerKey = ConfigurationManager.AppSettings.Get("twitter:ConsumerKey");
        private string _consumerSecret = ConfigurationManager.AppSettings.Get("twitter:ConsumerSecret");
        private string _accessKey = ConfigurationManager.AppSettings.Get("twitter:AccessKey");
        private string _accessToken = ConfigurationManager.AppSettings.Get("twitter:AccessToken");

        // Streams n' threads
        private IFilteredStream _filteredStream;
        private Thread _thread;

        public TwitterStream()
        {
            // call StreamsAhoy() below
            StreamsAhoy();
        }
        public void SampleStream()
        {
            // create sample stream obj if filtered stream has been rate limited
            var _sampleStream = Stream.CreateSampleStream();

            _sampleStream.FilterTweetsToBeIn(Language.English);

            _sampleStream.TweetReceived += (sender, args) =>
            {
                var tweet = args.Tweet;
                if (tweet != null)
                {
                    // broadcast sample stream tweet
                    //if (tweet.Coordinates.Latitude != null)
                    //{
                        // has latitude
                        var client = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;

                    //}
                }
            };
        }
        public void StreamsAhoy()
        {
            // start the stream, establish a remote connection to the hub and return to the client in a nice format
            var client = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;
            TwitterCredentials.SetCredentials(_accessKey, _accessToken, _consumerKey, _consumerSecret);

            // create the stream
            _filteredStream = Stream.CreateFilteredStream();

            // create instance of Coordinates class for top left and bottom right of world bounding box
            var topLeft = new Coordinates(-180, -90);
            var bottomRight = new Coordinates(180, 90);

            // add filters
            _filteredStream.AddLocation(topLeft, bottomRight);
            _filteredStream.FilterTweetsToBeIn(Language.English);

            // monitor tweets received and broadcast to the SignalR Hub class
            _filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                var tweet = args.Tweet;
                if (tweet != null) { 
                // pass complex TweetModel object to the client
                    client.All.broadcastTweetMessage(new TweetModel()
                    {
                        Text = tweet.Text,
                        User = tweet.Creator.ScreenName,
                        Latitude = tweet.Coordinates.Latitude,
                        Longitude = tweet.Coordinates.Longitude,
                        CreatedAt = tweet.CreatedAt,
                        ImageUrl = tweet.Creator.ProfileImageUrl,
                        Length = tweet.Length
                    });
                }
                else
                {
                    var rateLimited = RateLimit.GetCurrentCredentialsRateLimits();
                    client.All.rateLimited(rateLimited.ToString());
                }
            };

            // create thread with stream matching all conditions, keeping it open

            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);

            // start the thread

            _thread.Start();
        }
        public void NotifyIfRateLimited()
        {
            // tweetinvi has a rate limiter class but not volume limiting class - need to write custom logic to detect that 1% is being reached. Or alternatively read headers
            
        }
    }
}