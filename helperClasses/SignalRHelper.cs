using FinalUniProject.Hubs;
using Microsoft.AspNet.SignalR;
using FinalUniProject.Models;
using FinalUniProject.NERModels;
using Microsoft.AspNet.SignalR.Hubs;

namespace FinalUniProject
{
    public static class SignalRHelper
    {
        private static IHubConnectionContext clients = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;
        public static void Debug(string message) 
        {
            clients.All.debug(message);
        }
        public static void Broadcast(Tweet tweet)
        {
            SignalRUsers.Users.ForEach(user =>
            {
                // for each connected signalr user, get their location and check bounds, then broadcast
                var usersBounds = user.userBoundingBox;
                if (usersBounds != null)
                {
                    if (GeoHelper.IsInBounds(tweet.Latitude, tweet.Longitude, user.ConnectionId))
                    {
                        if (user.isStreamRunning)
                        {
                            clients.Client(user.ConnectionId).broadcastTweetMessage(tweet);
                        }
                    }
                }
                else
                {
                    if (user.isStreamRunning)
                    {
                        // no bounds set, so therefore nationwide for this user
                        clients.Client(user.ConnectionId).broadcastTweetMessage(tweet);
                    }
                }
            });
        }
        public static void Broadcast(Entity<Tweet> entity)
        {
            SignalRUsers.Users.ForEach(user =>
            {
                // for each connected signalr user, get their location and check bounds, then broadcast
                //var usersBounds = user.userBoundingBox;
                //if (usersBounds != null)
                //{
                //    if (GeoHelper.IsInBounds(entity.averageCenter.Latitude, entity.averageCenter.Longitude, user.ConnectionId))
                //    {
                //        if (user.isStreamRunning)
                //        {
                //            clients.Client(user.ConnectionId).broadcastTrend(entity);
                //        }
                //    }
                //}
                //else
                //{
                    if (user.isStreamRunning)
                    {
                        // no bounds set, so therefore nationwide trends for this user
                        clients.All.broadcastTrend(entity);
                    }
                //}
            });
        }
    }
}