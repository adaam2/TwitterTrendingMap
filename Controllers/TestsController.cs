using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinalUniProject.TwitterLogic;
using edu.stanford.nlp.parser;
using java.util;
using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.pipeline;
using edu.stanford.nlp.util;
using System.Net;
using System.IO;
using HtmlAgilityPack;
using FinalUniProject.NERModels;
using FinalUniProject.IKVM.Extensions;
using FinalUniProject.helperClasses;
using edu.stanford.nlp.ling;
using System.Configuration;

namespace FinalUniProject.Controllers
{
    public class TestsController : Controller
    {
        //public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");

        // Create CRFClassifier obj (i.e. NER) using correct model
        public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.muc.7class.distsim.crf.ser.gz");
        public static string connectionString = ConfigurationManager.AppSettings.Get("main");

        // GET: /Tests/

        public ActionResult Index()
        {
            // Classify the text
            var classified = _classifier.classifyWithInlineXML("While the Scottish Enlightenment is traditionally considered to have concluded toward the end of the 18th century,[84] disproportionately large Scottish contributions to British science and letters continued for another 50 years or more, thanks to such figures as the physicists James Clerk Maxwell and Lord Kelvin, and the engineers and inventors James Watt and William Murdoch, whose work was critical to the technological developments of the Industrial Revolution throughout Britain.[85] In literature the most successful figure of the mid-19th century was Walter Scott. His first prose work, Waverley in 1814, is often called the first historical novel.[86] It launched a highly successful career that probably more than any other helped define and popularise Scottish cultural identity.[87] In the late 19th century, a number of Scottish-born authors achieved international reputations, such as Robert Louis Stevenson, Arthur Conan Doyle, J. M. Barrie and George MacDonald.[88] Scotland also played a major part in the development of art and architecture. The Glasgow School, which developed in the late 19th century, and flourished in the early 20th century, produced a distinctive blend of influences including the Celtic Revival the Arts and Crafts Movement, and Japonisme, which found favour throughout the modern art world of continental Europe and helped define the Art Nouveau style. Proponents included architect and artist Charles Rennie Mackintosh");

            // Establish reference to AnswerAnnotation class to pass in later to CoreLabel.get(key)
            CoreAnnotations.AnswerAnnotation ann = new CoreAnnotations.AnswerAnnotation();

            Response.Write(classified);
            //// Convert java.util.List to C# List of ArrayList of CoreLabel
            //List<ArrayList> list = CollectionExtensions.ToList<ArrayList>(classified);
            //list.ForEach(item =>
            //{
            //    var arr = item;
            //    // Inner loop - there may be more than one CoreLabel for each word in the sentence. i.e. a word is a PLACE and a PERSON.
            //    foreach (CoreLabel i in arr)
            //    {
            //        string originalText = i.originalText();
            //        object entityType = i.get(ann.getClass());
            //        switch (entityType.ToString())
            //        {
            //            case "PERSON":
            //                // create new PERSON entity and add to tweetmodel entitycollection
            //                break;
            //            case "LOCATION":
            //                // create new LOCATION entity and add to tweetmodel entitycollection
            //                break;
            //            case "ORGANIZATION":
            //                // create new ORGANIZATION entity and ditto the above
            //                break;
            //            case "TIME":
            //                break;
            //            case "MONEY":
            //                // is money necessary?
            //                break;
            //            case "PERCENT":
            //                // will people be using percent, if so: is this something I want to capture?
            //                break;
            //            case "DATE":
            //                // could be useful 4/20?
            //                break;
            //            default:
            //                // do error stuff or ignore
            //                break;
            //        }
            //    }
            //});
            return View();
        }
    }
}
