using FinalUniProject.Hubs;
using FinalUniProject.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Configuration;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Interfaces.Streaminvi;
using Tweetinvi.Logic.Model;

namespace FinalUniProject.TwitterLogic
{
    public class TwitterStream
    {
        // Consumer Keys + Secrets
        private string _consumerKey = ConfigurationManager.AppSettings.Get("twitter:ConsumerKey");
        private string _consumerSecret = ConfigurationManager.AppSettings.Get("twitter:ConsumerSecret");
        // Twitter OAuth Credentials
        private string _accessKey = ConfigurationManager.AppSettings.Get("twitter:AccessKey");
        private string _accessToken = ConfigurationManager.AppSettings.Get("twitter:AccessToken");

        // Streams n' threads
        private IFilteredStream _filteredStream;
        private Thread _thread;

        // Constructor
        public TwitterStream()
        {
            StreamsAhoy();
        }
        public void StreamsAhoy()
        {
            // Start the stream, establish a remote connection to the hub and return to the client in a nice format
            var client = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;
            TwitterCredentials.SetCredentials(_accessKey, _accessToken, _consumerKey, _consumerSecret);

            // Create the stream
            _filteredStream = Stream.CreateFilteredStream();

            // Create instance of Coordinates class for top left and bottom right of world bounding box
            //var topLeft = new Coordinates(-180, -90);
            //var bottomRight = new Coordinates(180, 90);

            // United Kingdom Bounding Box Points - Approximate
            var topLeft = new Coordinates(-8.164723,49.955269);
            var bottomRight = new Coordinates(1.7425, 60.6311);

            // Add filters
            _filteredStream.AddLocation(topLeft, bottomRight);
            _filteredStream.FilterTweetsToBeIn(Language.English);
      

            // Monitor tweets received and broadcast to client function
            _filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                var tweetargs = args.Tweet;
                if (tweetargs != null) {
                    TweetModel model = new TweetModel()
                        {
                            Text = tweetargs.Text.ToString().ParseURL().ParseHashtag().ParseUsername(),
                            User = tweetargs.Creator.ScreenName,
                            Latitude = tweetargs.Coordinates.Latitude,
                            Longitude = tweetargs.Coordinates.Longitude,
                            CreatedAt = tweetargs.CreatedAt,
                            ImageUrl = tweetargs.Creator.ProfileImageUrl,
                            ProfileUrl = "https://twitter.com/" + tweetargs.Creator.ScreenName
                        };
                // pass complex TweetModel object to the client
                    client.All.broadcastTweetMessage(model);

                    TweetParser parse = new TweetParser(model);
                }
            };

            // Lambda function with SignalR client broadcast function to detect if stream has ground to a halt. If this is the case, exception details are sent to the client and stream is restarted matching prior conditions
            _filteredStream.StreamStopped += (sender, args) =>
            {
                // instantiate custom TwitterException class instance and send to client
                client.All.broadcastStatus(new TwitterException()
                {
                    Message = !String.IsNullOrEmpty(args.Exception.Message) ? args.Exception.Message : "no exception message",
                    StackTrace = !String.IsNullOrEmpty(args.Exception.StackTrace) ? args.Exception.StackTrace : "no stack trace",
                    TwitterCode = args.DisconnectMessage.Code != 0 ? args.DisconnectMessage.Code : 0,
                    TwitterReason = !String.IsNullOrEmpty(args.DisconnectMessage.Reason) ? args.DisconnectMessage.Reason : "no twitter reason"
                });
                // Put the current Thread to sleep for 2 seconds
                Thread.Sleep(2000); //2500?

                // Restart Stream matching all conditions
                _filteredStream.StartStreamMatchingAllConditions();
            };
           
            _filteredStream.StreamStarted += (sender, args) =>
            {
                client.All.twitterConnectionSuccess("Connected to Twitter API");
            };
            _filteredStream.StreamResumed += (sender, args) =>
            {
                client.All.twitterConnectionSuccess("Connected to Twitter API");
            };
            // create thread with stream matching all conditions, keeping it open
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);

            // start the thread
            _thread.Start();
        }
    }
}