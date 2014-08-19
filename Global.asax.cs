﻿using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Threading.Tasks;
using System;
using FinalUniProject.TwitterLogic;
using Hangfire;

namespace FinalUniProject
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Create Task for the stream of tweets
            Task.Factory.StartNew(() => TwitterStream.Setup());

            // Remove entities from the static collection that haven't been updated in a while - i.e. preserving freshness of trends
            //System.Threading.Timer timer = new System.Threading.Timer(e => TweetParser.RemoveOldEntities(), null, TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(5));
            //timer.s

            //// Start task to save top entities into the db
            //var timer2 = new System.Threading.Timer(e => TweetParser.SaveTopEntities(), null, new TimeSpan(0,1,0), new TimeSpan(0,2,0));

        }
    }
}