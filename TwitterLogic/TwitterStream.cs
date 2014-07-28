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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace FinalUniProject.TwitterLogic
{
    public static class TwitterStream
    {
        // United Kingdom Bounding Box coordinates - Default View
        public static Coordinates topLeft = new Coordinates(-8.164723, 49.955269);
        public static Coordinates bottomRight = new Coordinates(1.7425, 60.6311);

        // global group name
        private static string globalStreamGroupName = "Global";

        // Bounds as set by the user when they perform any action on the map (zoom_changed, drag_end etc)
        public static BoundingBoxPoint userSetBounds;

        // Consumer Keys + Secrets
        private static string _consumerKey = ConfigurationManager.AppSettings.Get("twitter:ConsumerKey");
        private static string _consumerSecret = ConfigurationManager.AppSettings.Get("twitter:ConsumerSecret");
        // Twitter OAuth Credentials
        private static string _accessKey = ConfigurationManager.AppSettings.Get("twitter:AccessKey");
        private static string _accessToken = ConfigurationManager.AppSettings.Get("twitter:AccessToken");

        // Hub Context
        public static IHubContext context = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>();

        // Streams n' threads
        private static IFilteredStream _filteredStream = Stream.CreateFilteredStream();
        private static Thread _thread;

        private static TimeSpan _recommendedBackoffTimeout = new TimeSpan(0, 0, 240); // 240 seconds

        // Static constructor and dummy method
        public static void Setup()
        {
            // dummy method for static constructor
        }
        static TwitterStream()
        {
            // Start the stream, establish a remote connection to the hub and return to the client in a nice format
            var clients = context.Clients;

            TwitterCredentials.SetCredentials(_accessKey, _accessToken, _consumerKey, _consumerSecret);

            // Add filters
            _filteredStream.AddLocation(topLeft, bottomRight);
            _filteredStream.FilterTweetsToBeIn(Language.English);

            ExceptionHandler.WebExceptionReceived += (sender, args) =>
            {
                // This works.. woo
                // Prints out nice(r) view of web exception sent by Twitter API including error code response
                var exc = (ITwitterException) args.Value;
                clients.All.broadcastLog(args.Value.ToString());
                //Debug.WriteLine(exc.ToString());
                /* List of Status Codes available below:
                 * https://dev.twitter.com/docs/error-codes-responses
                 * Main status codes that we will need to filter on in the switch statement below
                 */
                var statusCode = exc.StatusCode;
                if (statusCode != 200)
                {
                    clients.All.broadcastLog("Twitter API error " + statusCode);
                    switch (statusCode)
                    {
                        case 420:
                        case 429:
                            // enhance your calm - being rate limited
                            if (!(Thread.CurrentThread.ThreadState == System.Threading.ThreadState.WaitSleepJoin)) { 
                                Thread.Sleep(_recommendedBackoffTimeout);
                            }
                            
                            break;
                    }
                }
            };
            // Monitor tweets received and broadcast to client function
            _filteredStream.MatchingTweetReceived += (sender, args) =>
            {
                var tweetargs = args.Tweet;
                if (tweetargs != null)
                {
                    if (!tweetargs.IsRetweet)
                    {
                        FinalUniProject.Models.Tweet tweet = new FinalUniProject.Models.Tweet()
                        {
                            Text = tweetargs.Text.ToString().ParseURL().ParseHashtag().ParseUsername().TrimWhitespace(),
                            User = tweetargs.Creator.ScreenName,
                            Latitude = tweetargs.Coordinates.Latitude,
                            Longitude = tweetargs.Coordinates.Longitude,
                            CreatedAt = tweetargs.CreatedAt,
                            ImageUrl = tweetargs.Creator.ProfileImageUrl,
                            ProfileUrl = "https://twitter.com/" + tweetargs.Creator.ScreenName
                        };
                        // pass complex TweetModel object to the client

                        if (userSetBounds != null)
                        {
                            if (IsInBounds(tweet.Latitude, tweet.Longitude))
                            {
                                clients.Group(globalStreamGroupName).broadcastTweetMessage(tweet);
                            }
                        }
                        else
                        {
                            clients.Group(globalStreamGroupName).broadcastTweetMessage(tweet);
                        }


                        //Task.Factory.StartNew(() => TweetParser.ProcessTweet(tweet));
                    }
                }
            };

            // Lambda function with SignalR client broadcast function to detect if stream has ground to a halt. If this is the case, exception details are sent to the client and stream is restarted matching prior conditions
            //_filteredStream.StreamStopped += (sender, args) =>
            //{
            //    //// Put the current Thread to sleep for 2 seconds
            //    //Thread.Sleep(10000);

            //    // Restart Stream matching all conditions
            //    _filteredStream.StartStreamMatchingAllConditions();
            //};

            // create thread with stream matching all conditions, keeping it open
            _thread = new Thread(_filteredStream.StartStreamMatchingAllConditions);

            // start the thread
            _thread.Start();
        }

        // Refactor these into geometry singleton
        public static BoundingBoxPoint GetUserSetBounds()
        {
            return userSetBounds;
        }
        public static void SetUserBounds(BoundingBoxPoint points)
        {
            userSetBounds = points;
        }
        public static bool IsInBounds(double lat, double lng)
        {
            if ((lat <= userSetBounds.NorthWestLatitude && lat >= userSetBounds.SouthEastLatitude) && (lng >= userSetBounds.NorthWestLongitude && lng <= userSetBounds.SouthEastLongitude))
            {
                // The point is in the box
                return true;
            }
            return false;
        }
    }
}