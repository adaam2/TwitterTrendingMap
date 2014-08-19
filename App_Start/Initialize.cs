using Owin;
using Newtonsoft.Json;
using Microsoft.AspNet.SignalR;
using Hangfire;
using Hangfire.SqlServer;
using System.Configuration;

[assembly:Microsoft.Owin.OwinStartup(typeof(FinalUniProject.App_Start.Initialize))]
namespace FinalUniProject.App_Start
{
    public partial class Initialize
    {
        // This class simply initiates the Owin connection for SignalR
        public void Configuration(IAppBuilder app)
        {

            // Register Hangfire Task Scheduler

            app.UseHangfire(config =>
            {
                // Basic setup required to process background jobs.
                config.UseSqlServerStorage(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString);
                config.UseAuthorizationFilters();
                config.UseServer();
            });

            // Prevent TweetModel/NamedEntity endless loop during Json.Net serialization
            var serializerSettings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };
            // Register Newtonsoft Json.net Serializer settings with SignalR Globalhost
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(serializerSettings));
            
            // Register Hubs
            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
         
            app.MapSignalR(hubConfiguration);

           
        }
    }
}