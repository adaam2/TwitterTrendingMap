using Tweetinvi.Core.Interfaces.Streaminvi;
using System.Threading;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using FinalUniProject.Hubs;
using Tweetinvi;
using FinalUniProject.Models;
using Tweetinvi.Logic.Model;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Exceptions;

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
                        ProfileUrl = "https://twitter.com/" + tweet.Creator.ScreenName
                    });
                }
            };

            // Lambda function with SignalR client broadcast function to detect if stream has ground to a halt. If this is the case, exception details are sent to the client and stream is restarted matching prior conditions
            _filteredStream.StreamStopped += (sender, args) =>
            {
                var exceptionDescription = ExceptionHandler.GetLastException().TwitterDescription;
                // instantiate custom TwitterException class instance and send to client
                client.All.broadcastStatus(new TwitterException() {
                    Message = args.Exception.Message,
                    StackTrace = args.Exception.StackTrace
                });

                // Put the current Thread to sleep for 2 seconds
                Thread.Sleep(2000);

                // Restart Stream
                _filteredStream.StartStreamMatchingAllConditions();
            };

            // create thread with stream matching all conditions, keeping it open
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);

            // start the thread
            _thread.Start();
        }
    }
}