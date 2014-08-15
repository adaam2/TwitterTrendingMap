using Microsoft.AspNet.SignalR;
using FinalUniProject.Hubs;
using System.ComponentModel;
using System;
using Microsoft.AspNet.SignalR.Hubs;

namespace FinalUniProject
{
    /// <summary>
    /// This static class tracks status of Rate Limits and broadcasts to the hub if the property is changed
    /// </summary>
    public static class TwitterStatusHelper
    {
        public static IHubConnectionContext clients = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;
        private static bool _isRateLimited = false;
        public static bool isRateLimited
        {
            get
            {
                return _isRateLimited;
            }
            set
            {
                _isRateLimited = value;
                RateLimitChanged();
            }
        }
        private static void RateLimitChanged()
        {
                while (isRateLimited)
                {
                    clients.All.broadcastLog("Now being rate limited");
                }
        }
    }
}