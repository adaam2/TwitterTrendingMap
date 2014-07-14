using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinalUniProject.TwitterLogic
{
    public class TwitterException
    {
        public string StackTrace { get; set; }
        public string Message { get; set; }
        //public string InnerException { get; set; }
        //public string TargetSite { get; set; }
        public int TwitterCode { get; set; }
        public string TwitterReason { get; set; }
    }
}