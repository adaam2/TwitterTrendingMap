using Tweetinvi;
using Tweetinvi.Core.Interfaces;
using Tweetinvi.Core.Interfaces.oAuth;
using FinalUniProject.Models;
using Microsoft.AspNet.SignalR;

namespace FinalUniProject
{
    public class TwitterUser
    {
        public string Name { get; set; }
        public string Location { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ProfileUrl { get; set; }
        public IOAuthCredentials Credentials { get; set; }
        public bool IsDefaultProfileImage { get; set; }
        public BoundingBoxPoint userBoundingBox { get; set; }
    }
}