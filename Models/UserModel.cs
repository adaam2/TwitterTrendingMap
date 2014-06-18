using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalUniProject.Models
{
    public class UserModel
    {
        public string ScreenName { get; set; }
        public string Location { get; set; }
        public string ProfileImageUrl { get; set; }
        public string ProfileBackgroundImageUrl { get; set; }
        public int NumberOfFollowers { get; set; }
    }
}