using System.Web.Mvc;
using FinalUniProject.TwitterLogic;
using FinalUniProject.NERModels;
using FinalUniProject.Models;
using FinalUniProject.helperClasses;
using System.Collections.Generic;
using System.Data;

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

            // do percentages
            string sql = @"select dbo.EntityTypes.entityTypeName as [Type], CEILING(100. * count(*) / sum(count(*)) over ()) as Percentage from dbo.Entities join dbo.EntityTypes on dbo.Entities.entityTypeID = dbo.EntityTypes.ID group by dbo.EntityTypes.entityTypeName order by Percentage desc";

            var dt = Database.GetAsDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                List<PercentagesByType> list = new List<PercentagesByType>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    PercentagesByType typePercentageStat = new PercentagesByType();
                    typePercentageStat.Type = dt.Rows[i]["Type"].ToString();
                    typePercentageStat.Percentage = decimal.Parse(dt.Rows[i]["Percentage"].ToString());

                    list.Add(typePercentageStat);
                }
                ViewBag.Percentages = list;
            }

            string sql2 = @"select count(distinct(Tweets.tweetEncodedText)) as Count from dbo.Tweets";
            var dt2 = Database.GetAsDataTable(sql2);

            if (dt2.Rows.Count > 0)
            {
                TweetCount count = new TweetCount();
                count.Count = int.Parse(dt2.Rows[0]["Count"].ToString());
                ViewBag.TweetCount = count;
            }
            
            return View();
        }
    }
    public class PercentagesByType
    {
        public string Type { get; set; }
        public decimal Percentage { get; set; }
    }
    public class TweetCount
    {
        public int Count { get; set; }
    }
}
