﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Configuration;
using Tweetinvi;
using Tweetinvi.Streams;
using Microsoft.AspNet.SignalR;
using FinalUniProject.TwitterLogic;
using Tweetinvi.Core.Interfaces.Streaminvi;
using System.Threading.Tasks;
using FinalUniProject.helperClasses;
using System.Diagnostics;
using FinalUniProject.Models;
using Tweetinvi.Logic.Model;

namespace FinalUniProject.Hubs
{
    public class GeoFeedHub : Hub
    {
        public void ChangeStreamBounds(Coordinates sw)
        {
            Debug.WriteLine(sw.Latitude + sw.Longitude);
            //FinalUniProject.TwitterLogic.TwitterStream.ChangeStreamBounds(southWest, northEast);
        }
        public override Task OnConnected()
        {
            // get ip and browser
            // add connection id to hashset
            ClientsConnected.ConnectedClients.Add(Context.ConnectionId);
            Clients.All.getClientsConnectedCount(ClientsConnected.ConnectedClients);
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            // remove the client connection id from the hashset
            ClientsConnected.ConnectedClients.Remove(Context.ConnectionId);
            Clients.All.getClientsConnectedCount(ClientsConnected.ConnectedClients);
            return base.OnDisconnected();
        }
        public override Task OnReconnected()
        {
            // for reconnections
            ClientsConnected.ConnectedClients.Add(Context.ConnectionId);
            Clients.All.getClientsConnectedCount(ClientsConnected.ConnectedClients);
            return base.OnReconnected();
        }
    }
}