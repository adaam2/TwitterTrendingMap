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
        public override Task OnConnected()
        {
            // add connection id to hashset
            ClientsConnected.ConnectedIds.Add(Context.ConnectionId);
            Clients.All.getClientsConnectedCount(ClientsConnected.ConnectedIds.Count);
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            // remove the client connection id from the hashset
            ClientsConnected.ConnectedIds.Remove(Context.ConnectionId);
            Clients.All.getClientsConnectedCount(ClientsConnected.ConnectedIds.Count);
            return base.OnDisconnected();
        }
        public override Task OnReconnected()
        {
            // for reconnections
            ClientsConnected.ConnectedIds.Add(Context.ConnectionId);
            Clients.All.getClientsConnectedCount(ClientsConnected.ConnectedIds.Count);
            return base.OnReconnected();
        }
    }
}