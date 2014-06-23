using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Configuration;
using Tweetinvi;
using Tweetinvi.Streams;
using Microsoft.AspNet.SignalR;
using Tweetinvi.Core.Interfaces.Streaminvi;
using System.Threading.Tasks;
using FinalUniProject.helperClasses;

namespace FinalUniProject.Hubs
{
    public class GeoFeedHub : Hub
    {
        // Member Variables

        private bool _sessionEnabledThread;
        private Thread _thread;
        private ISampleStream _sampleStream;

        // Twitter Access Keys
        private string _consumerKey = ConfigurationManager.AppSettings.Get("twitter:ConsumerKey");
        private string _consumerSecret = ConfigurationManager.AppSettings.Get("twitter:ConsumerSecret");
        private string _accessKey = ConfigurationManager.AppSettings.Get("twitter:AccessKey");
        private string _accessToken = ConfigurationManager.AppSettings.Get("twitter:AccessToken");

        public void Send(string msg)
        {
            Clients.All.add(msg);
        }
        public override Task OnConnected()
        {
            // add connection id to hashset
            ClientsConnected.ConnectedIds.Add(Context.ConnectionId);
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            // remove the client connection id from the hashset
            ClientsConnected.ConnectedIds.Remove(Context.ConnectionId);
            return base.OnDisconnected();
        }
        public void getNumberOfClients()
        {
            Clients.All.numberOfClients(ClientsConnected.ConnectedIds.Count);
        }
    }
}