using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using Tweetinvi;
using Tweetinvi.Core.Interfaces.Credentials;

namespace FinalUniProject.Controllers
{
    public class MapController : Controller
    {
        // Consumer Keys + Secrets
        private string _consumerKey = ConfigurationManager.AppSettings.Get("twitter:ConsumerKey");
        private string _consumerSecret = ConfigurationManager.AppSettings.Get("twitter:ConsumerSecret");
        private ITemporaryCredentials _ApplicationCredentials;
        private static oAuthProcessCredentials _AuthorizationObj;

        // GET: /Map/
       public MapController()
        {
            _ApplicationCredentials = CredentialsCreator.GenerateApplicationCredentials(_consumerKey, _consumerSecret);
        }
       static MapController()
       {
           _AuthorizationObj = new oAuthProcessCredentials();
       }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        public ActionResult Authorize()
        {
            var url = CredentialsCreator.GetAuthorizationURLForCallback(_ApplicationCredentials,this.getCallBackURL());

            //Saving applicationCredentials into custom obj for using them later. 
            _AuthorizationObj.AuthorizationKey = _ApplicationCredentials.AuthorizationKey;
            _AuthorizationObj.AuthorizationSecret = _ApplicationCredentials.AuthorizationSecret;

            // Redirect to Twitter auth pg
            return new RedirectResult(url, false);
        }
        public ActionResult Callback(string oauth_token, string oauth_verifier)
        {
            if (!String.IsNullOrEmpty(oauth_verifier))
            {              
                //Setting applicationCredentials to the previous one i saved in sessions.
                _ApplicationCredentials.AuthorizationKey = _AuthorizationObj.AuthorizationKey;
                _ApplicationCredentials.AuthorizationSecret = _AuthorizationObj.AuthorizationSecret;

                // Here we provide the entire URL where the user has been redirected
                var newCredentials = CredentialsCreator.GetCredentialsFromCallbackURL(System.Web.HttpContext.Current.Request.RawUrl, _ApplicationCredentials);
                if (newCredentials != null)
                {
                    //use your credentials here newCredentials.AccessToken, newCredentials.AccessTokenSecret
                    Debug.WriteLine("sccuess");
                    var credentials = TwitterCredentials.CreateCredentials(newCredentials.AccessToken, newCredentials.AccessTokenSecret, _consumerKey, _consumerSecret);
                    TwitterCredentials.ExecuteOperationWithCredentials(credentials, () =>
                    {
                        var u = Tweetinvi.User.GetLoggedUser();
                        TwitterUser loggedInUser = new TwitterUser()
                        {
                            Credentials = u.Credentials,
                            IsDefaultProfileImage = u.DefaultProfileImage,
                            Location = u.Location,
                            Name = u.Name,
                            ProfileImageUrl = u.ProfileImageUrl,
                            ProfileUrl = u.Url       
                        };
                        TwitterUsers.Users.Add(loggedInUser);
                    });
                }
            }

            return RedirectToAction("Index", "Map");
        }
        public string getCallBackURL()
        {
            return "http://10.211.55.7/Map/Callback";
        }
    }
}
