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

            // Classify the text
            List classified = _classifier.classify("While the Scottish Enlightenment is traditionally considered to have concluded toward the end of the 18th century,[84] disproportionately large Scottish contributions to British science and letters continued for another 50 years or more, thanks to such figures as the physicists James Clerk Maxwell and Lord Kelvin, and the engineers and inventors James Watt and William Murdoch, whose work was critical to the technological developments of the Industrial Revolution throughout Britain.[85] In literature the most successful figure of the mid-19th century was Walter Scott. His first prose work, Waverley in 1814, is often called the first historical novel.[86] It launched a highly successful career that probably more than any other helped define and popularise Scottish cultural identity.[87] In the late 19th century, a number of Scottish-born authors achieved international reputations, such as Robert Louis Stevenson, Arthur Conan Doyle, J. M. Barrie and George MacDonald.[88] Scotland also played a major part in the development of art and architecture. The Glasgow School, which developed in the late 19th century, and flourished in the early 20th century, produced a distinctive blend of influences including the Celtic Revival the Arts and Crafts Movement, and Japonisme, which found favour throughout the modern art world of continental Europe and helped define the Art Nouveau style. Proponents included architect and artist Charles Rennie Mackintosh");

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
                    Response.Write("Entity: " + value + "<br/>"); // can turn this into a function and return the value as a string to the NamedEntity intiializer
                }
                
            }
            
            //for (int i = 0; i < classified.Length; i++)
            //{
            //    Triple triple = (Triple)classified[i];

            //    int second = Convert.ToInt32(triple.second().ToString());
            //    int third = Convert.ToInt32(triple.third().ToString());

            //    Response.Write(triple.first().ToString() + triple.second().ToString() + triple.third().ToString() + Environment.NewLine);
            //}

                return View();
        }
    }
}
