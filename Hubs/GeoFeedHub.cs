using System;
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
        private static string[] _reservedGroupNames = new[] { "Global" };
        public bool SubscribeToStreamGroup(string groupName)
        {
            if (_reservedGroupNames.Any(x => x.Equals(groupName, StringComparison.OrdinalIgnoreCase)))
            {
                var user = SignalRUsers.Users.Find(e => e.ConnectionId == Context.ConnectionId);
                user.isStreamRunning = true;
                Groups.Add(Context.ConnectionId, groupName);
                return true;
            }
            return false;
        }
        public void UnsubscribeFromStreamGroup(string groupName)
        {
            if (_reservedGroupNames.Any(x => x.Equals(groupName, StringComparison.OrdinalIgnoreCase)))
            {
                var user = SignalRUsers.Users.Find(e => e.ConnectionId == Context.ConnectionId);
                user.isStreamRunning = false;
                Groups.Remove(Context.ConnectionId, groupName);
            }
        }
        //public void KillStream()
        //{
        //    FinalUniProject.TwitterLogic.TwitterStream.KillStream();
        //}
        public void ChangeStreamBounds(BoundingBoxPoint points, string connectionId)
        {
            GeoHelper.SetUserBounds(points,Context.ConnectionId);
        }
        public override Task OnConnected()
        {
            SignalRUsers.Users.Add(new SignalRUser()
            {
                ConnectionId = Context.ConnectionId
            });
            return base.OnConnected();
        }
        public override Task OnDisconnected()
        {
            var userToBeRemoved = SignalRUsers.Users.Find(e => e.ConnectionId == Context.ConnectionId);
            SignalRUsers.Users.Remove(userToBeRemoved);
            return base.OnDisconnected();
        }
    }
}