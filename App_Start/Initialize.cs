using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Web.Routing;
using Owin;
using FinalUniProject.Hubs;
using Microsoft.AspNet.SignalR;


[assembly:Microsoft.Owin.OwinStartup(typeof(FinalUniProject.App_Start.Initialize))]
namespace FinalUniProject.App_Start
{
    public class Initialize
    {
        // This class simply initiates the Owin connection for SignalR
        public static void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            app.MapSignalR(hubConfiguration);
           
        }
    }
}