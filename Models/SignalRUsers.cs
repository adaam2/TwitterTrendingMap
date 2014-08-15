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
    public static class SignalRUsers
    {
        public static IHubContext context;
        public static List<SignalRUser> Users;
        
        static SignalRUsers()
        {
            context = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>();
            Users = new List<SignalRUser>();
        }
        public static void AddUser(SignalRUser user)
        {
            Users.Add(user);
            BroadcastNewUserToHub(user);
        }
        public static void RemoveUser(SignalRUser user)
        {
            Users.Remove(user);
        }
        public static List<SignalRUser> AsListOfT()
        {
            return Users;
        }
        public static IEnumerable AsEnumerable()
        {
            return Users.AsEnumerable();
        }
        public static IEnumerable<SignalRUser> Where(Func<SignalRUser, bool> predicate)
        {
            return Users.Where<SignalRUser>(predicate);
        }
        public static void BroadcastNewUserToHub(SignalRUser user)
        {            
            context.Clients.All.BroadcastNewUserToHub(user);
        }
    }
}