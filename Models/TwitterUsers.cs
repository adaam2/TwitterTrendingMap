using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Web;
using FinalUniProject.Hubs;
using System.Threading;

namespace FinalUniProject
{
    public static class TwitterUsers
    {
        public static IHubContext context;
        public static List<TwitterUser> Users;
        
        static TwitterUsers()
        {
            context = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>();
            Users = new List<TwitterUser>();
        }
        public static void AddUser(TwitterUser user)
        {
            Users.Add(user);
            BroadcastNewUserToHub(user);
        }
        public static void RemoveUser(TwitterUser user)
        {
            Users.Remove(user);
        }
        public static List<TwitterUser> AsListOfT()
        {
            return Users;
        }
        public static IEnumerable AsEnumerable()
        {
            return Users.AsEnumerable();
        }
        public static IEnumerable<TwitterUser> Where(Func<TwitterUser, bool> predicate)
        {
            return Users.Where<TwitterUser>(predicate);
        }
        public static void BroadcastNewUserToHub(TwitterUser user)
        {
            
            context.Clients.All.BroadcastNewUserToHub(user);
        }
    }
}