using FinalUniProject.Hubs;
using FinalUniProject.Models;
using Microsoft.AspNet.SignalR;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;
using System.Web;
using Tweetinvi;
using Tweetinvi.Core.Enum;
using Tweetinvi.Core.Exceptions;
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
            var topLeft = new Coordinates(-8.164723, 49.955269);
            var bottomRight = new Coordinates(1.7425, 60.6311);

            // Add filters
            _filteredStream.AddLocation(topLeft, bottomRight);
            _filteredStream.FilterTweetsToBeIn(Language.English);

            ExceptionHandler.WebExceptionReceived += (sender, args) =>
            {
                // This works.. woo
                // Prints out nice(r) view of web exception sent by Twitter API including error code response
                var exc = (ITwitterException)args.Value;
                Debug.WriteLine(exc);
                /* List of Status Codes available below:
                 * https://dev.twitter.com/docs/error-codes-responses
                 * Main status codes that we will need to filter on in the switch statement below
                 */
                var statusCode = exc.StatusCode;
                if (statusCode != 200)
                {
                    //switch (statusCode)
                    //{
                    //    case 420:
                    //    case 429:
                    //        // enhance your calm - being rate limited
                            
                    //        break;
                    //    case 503:
                    //        // twitter servers up - but too many global requests - show error message

                    //        break;
                    //    case 500:
                    //    case 502:
                    //    case 504:
                    //        // twitter servers down
                    //        // show error message
                    //        break;
                    //}
                    
                }
            };
            // Monitor tweets received and broadcast to client function
            _filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                var tweetargs = args.Tweet;
                if (tweetargs != null)
                {
                    FinalUniProject.Models.Tweet tweet = new FinalUniProject.Models.Tweet()
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
                    client.All.broadcastTweetMessage(tweet);

                    TweetParser.ProcessTweet(tweet);
                }
            };

            // Lambda function with SignalR client broadcast function to detect if stream has ground to a halt. If this is the case, exception details are sent to the client and stream is restarted matching prior conditions
            _filteredStream.StreamStopped += (sender, args) =>
            {
                // Put the current Thread to sleep for 2 seconds
                Thread.Sleep(2500); //2500?

                // Restart Stream matching all conditions
                _filteredStream.StartStreamMatchingAllConditions();
            };

            // create thread with stream matching all conditions, keeping it open
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);

            // start the thread
            _thread.Start();
        }
    }
}