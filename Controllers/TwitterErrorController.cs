using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalUniProject.Controllers
{
    /// <summary>
    /// Controller to serve error view when a Twitter WebException occurs on the map page. 
    /// </summary>
    public class TwitterErrorController : Controller
    {
        //
        // GET: /TwitterError/

        public ActionResult Index(int? twitterCode = null)
        {
            string errorMessage = String.Empty;
            switch (twitterCode)
            {
                case 420:
                case 429:
                    // enhance your calm - being rate limited
                    errorMessage = "The application is currently being rate limited by the Twitter API. Please come back another time.";
                    break;
                case 503:
                    // twitter servers up - but too many global requests - show error message
                    errorMessage = "The Twitter servers are receiving too many global requests. Please try again later.";
                    break;
                case 500:
                case 502:
                case 504:
                    // twitter servers down
                    // show error message
                    errorMessage = "Twitter's servers are down :(";
                    break;
                default:
                    errorMessage = "Sorry, an error has occured. An email has been sent to the system administrator.";
                    break;
            }
            ViewBag.errorMessage = errorMessage;
            return View();
        }

    }
}
