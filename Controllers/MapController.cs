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
            return View();
        }
        public ActionResult Statistics()
        {
            ViewBag.TopEntities = TweetParser.GetTopEntities();
            ViewBag.Organisations = TweetParser.GetTopEntities(TweetParser.EntityType.Organisation, 5);
            ViewBag.People = TweetParser.GetTopEntities(TweetParser.EntityType.Person, 5);
            ViewBag.Places = TweetParser.GetTopEntities(TweetParser.EntityType.Place, 5);
            ViewBag.LastUpdated = TweetParser.GetTopEntities(isLastUpdatedQuery: true);
            return View();
        }
    }
}
