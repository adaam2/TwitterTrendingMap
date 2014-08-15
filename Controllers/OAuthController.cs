using System;
using System.Collections.Generic;
using System.Linq;
using Tweetinvi;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Diagnostics;



namespace FinalUniProject.Controllers
{
    public class OAuthController : Controller
    {
        //public ActionResult Authorize()
        //{
        //    var CREDENTS1 = CredentialsCreator.GenerateApplicationCredentials("Ys0AtyHwywZnHxTQncDw28Ma5","suT4dSXQWvxrTcqmHhmH1j9gZBftAiudYgFfrOue0CZn3h611J");

        //    var url = CredentialsCreator.GetAuthorizationURLForCallback(CREDENTS1, this.GetCallbackUrl());
        //    return new RedirectResult(url, false);
        //}

        //// This URL is registered as the application's callback at http://dev.twitter.com
        //public ActionResult Callback(string oauth_token, string oauth_verifier)
        //{
        //    var CREDENTS2 = CredentialsCreator.GenerateApplicationCredentials("Ys0AtyHwywZnHxTQncDw28Ma5", "suT4dSXQWvxrTcqmHhmH1j9gZBftAiudYgFfrOue0CZn3h611J");
        //    var newCredentials = CredentialsCreator.GetCredentialsFromVerifierCode(oauth_verifier,CREDENTS2);
        //    return RedirectToAction("Index", "Map");    
        //}
        private string GetCallbackUrl()
        {
            return String.Format("{0}://{1}:{2}/oauth/callback", Request.Url.Scheme, Request.Url.Host, Request.Url.Port);
        }
    }
}
