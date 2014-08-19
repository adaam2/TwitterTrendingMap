using System.Web.Mvc;
using FinalUniProject.TwitterLogic;
using FinalUniProject.NERModels;
using FinalUniProject.Models;
using System.Collections.Generic;

namespace FinalUniProject.Controllers
{
    public class MapController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.TopEntities = TweetParser.GetTopEntities();
            return View();
        }
    }
}
