using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinalUniProject.Hubs;
using java.util;
using System.Threading.Tasks;
using System.Text;
using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.util;
using Microsoft.AspNet.SignalR;
using System.Data.SqlClient;
using FinalUniProject.helperClasses;
using edu.stanford.nlp.process;

namespace FinalUniProject.TwitterLogic
{
    public static class TweetParser
    {
        public static CRFClassifier classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");
        //public TweetParser()
        //{
        //    var client = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;
        //    //string testWiki = "http://en.wikipedia.org/wiki/Microsoft?action=raw";
            
        //    //client.All.returnParsed(test);
        //}
        public static string returnParsed()
        {
            string test = "Hello my name is Adam and I am from St Albans in Hertfordshire.";
            test = classifier.classifyWithInlineXML(test);
            return test;
        }
    }
}