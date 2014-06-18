using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalUniProject.TwitterLogic;

namespace FinalUniProject.Controllers
{
    public class TestsController : Controller
    {
        //
        // GET: /Tests/

        public ActionResult Index()
        {
            //ViewBag.Parsed = TweetParser.returnParsed();
            return View();
        }

    }
}
