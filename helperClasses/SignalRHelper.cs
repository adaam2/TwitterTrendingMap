using FinalUniProject.Hubs;
using Microsoft.AspNet.SignalR;
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
    }
}