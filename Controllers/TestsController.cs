using System.Collections.Generic;
using System.Web.Mvc;
using java.util;
using edu.stanford.nlp.ie.crf;
using FinalUniProject.IKVM.Extensions;
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
                return View();
        }
        [HttpPost]
        public void ParseEntities(string text)
        {
            // Classify the text
            List classified = _classifier.classify(text);

            // Establish reference to AnswerAnnotation class to pass in later to CoreLabel.get(key)
            CoreAnnotations.AnswerAnnotation ann = new CoreAnnotations.AnswerAnnotation();
            CoreAnnotations.BeforeAnnotation bef = new CoreAnnotations.BeforeAnnotation();
            CoreAnnotations.OriginalTextAnnotation orig = new CoreAnnotations.OriginalTextAnnotation();
            CoreAnnotations.AfterAnnotation aft = new CoreAnnotations.AfterAnnotation();
            CoreAnnotations.ValueAnnotation valueAnn = new CoreAnnotations.ValueAnnotation();
            Dictionary<string, Dictionary<string, int>> entities = new Dictionary<string, Dictionary<string, int>>();

            List<ArrayList> list = CollectionExtensions.ToList<ArrayList>(classified);
            string bg = _classifier.flags.backgroundSymbol;

            // Lifted this code below from http://stackoverflow.com/a/15680613/1795862
            // Converted (with great trepidation) from Java
            foreach (ArrayList inner in list)
            {
                Iterator it = inner.iterator();
                if (!it.hasNext()) continue;

                CoreLabel cl = (CoreLabel)it.next();
                while (it.hasNext())
                {
                    string answ = cl.getString(ann.getClass());
                    if (answ.Equals(bg))
                    {
                        cl = (CoreLabel)it.next();
                        continue;
                    }
                    if (!entities.ContainsKey(answ))
                    {
                        entities.Add(answ, new Dictionary<string, int>());
                    }
                    string value = cl.value();
                    while (it.hasNext())
                    {
                        cl = (CoreLabel)it.next();
                        if (answ.Equals(cl.getString(ann.getClass())))
                        {
                            value = value + " " + cl.getString(valueAnn.getClass());
                        }
                        else
                        {
                            if (!entities.ContainsKey(answ))
                            {
                                entities[answ].Add(value, 0);
                            }
                            Dictionary<string, int> innerDict = entities[answ];
                            int number;
                            if (innerDict.ContainsKey(value))
                            {
                                number = innerDict[value] + 1;
                                innerDict.Add(value, number);
                            }

                            break;
                        }

                    }
                    if (!it.hasNext())
                    {
                        break;
                    }
                    Response.Write(answ.ToUpper() + ": " + value + "<br/>"); // can turn this into a function and return the value as a string to the NamedEntity intiializer
                }

            }
            //return View();
        }
    }
}
