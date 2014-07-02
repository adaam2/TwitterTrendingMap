using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.ling;
using FinalUniProject.IKVM.Extensions;
using FinalUniProject.Models;
using FinalUniProject.NERModels;
using java.util;
using Microsoft.AspNet.SignalR;
using FinalUniProject.Hubs;
using System;

using System.Collections.Generic;

namespace FinalUniProject.TwitterLogic
{
    public class TweetParser
    {
        public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");
        public static StringTokenizer _tokenizer;
        //public static TweetCollection dict = new TweetCollection();
        public static System.Collections.Generic.List<TweetModel> tweets = new System.Collections.Generic.List<TweetModel>();
        public TweetParser(TweetModel tweet)
        {
            //var timer = new System.Threading.Timer(e => RemoveOldTweets(),null,TimeSpan.Zero,TimeSpan.FromMinutes(5));
            ProcessTweet(tweet);
        }
        private void ProcessTweet(TweetModel tweet)
        {
            //// Classify the text
            var classified = _classifier.classify(tweet.Text);

            // Establish reference to AnswerAnnotation class to pass in later to CoreLabel.get(key)
            CoreAnnotations.AnswerAnnotation ann = new CoreAnnotations.AnswerAnnotation();
            CoreAnnotations.BeforeAnnotation bef = new CoreAnnotations.BeforeAnnotation();
            CoreAnnotations.OriginalTextAnnotation orig = new CoreAnnotations.OriginalTextAnnotation();
            CoreAnnotations.AfterAnnotation aft = new CoreAnnotations.AfterAnnotation();

            // Convert java.util.List to C# List of ArrayList of CoreLabel
            List<ArrayList> list = CollectionExtensions.ToList<ArrayList>(classified);
            list.ForEach(item =>
            {
                var arr = item;
                string bg = _classifier.flags.backgroundSymbol;
                string prevType = "";
                string prevValue = "";

                foreach (CoreLabel i in arr)
                {
                    string type = i.get(ann.getClass()).ToString();
                    if (type != bg)
                    {
                        string value = i.originalText();
                        string className = "";
                        switch (type)
                        {
                            case "LOCATION":
                                className = "Place";
                                break;
                            case "PERSON":
                                className = "Person";
                                break;
                            case "ORGANIZATION":
                                className = "Organisation";
                                break;
                            default:
                                className = null;
                                break;
                        }
                        if (type == prevType)
                        {
                            if (className != null)
                            {
                                Type t = Type.GetType("FinalUniProject.NERModels." + className);
                                //Response.Write(t.ToString());

                                NamedEntity combined = (NamedEntity) Activator.CreateInstance(t);
                                combined.Name = prevValue + " " + value;
                                this.AddToEntityCollection(tweet, combined);
                            }
                        }
                        else
                        {
                            if (className != null) { 
                            Type t = Type.GetType("FinalUniProject.NERModels." + className);
                            NamedEntity single = (NamedEntity) Activator.CreateInstance(t);
                            this.AddToEntityCollection(tweet, single);
                            }
                        }
                        prevType = type;
                        prevValue = value;
                        //Response.Write("yes" + bg);
                    }
                }
            });
        }
        private void AddToEntityCollection(TweetModel tweet, NamedEntity entity)
        {
            if (entity.Name != null) { 
            if (tweet.entities.Count > 0)
            {
                tweet.entities.Add(entity);
            }
            else
            {
                tweet.entities = new List<NamedEntity>();
                tweet.entities.Add(entity);
            }
            BroadcastToHub(tweet.entities);
            }
        }
        private void BroadcastToHub(List<NamedEntity> entities)
        {
            var client = GlobalHost.ConnectionManager.GetHubContext<TrendsAnalysisHub>().Clients;
            client.All.broadcastTrend(entities);
        }
        //private void RemoveOldTweets()
        //{
        //    tweets.ForEach(tweet =>
        //        {
        //            var now = DateTime.Now;
        //            var then = tweet.CreatedAt;
        //            var difference = now.Subtract(then); // finding out the difference between the tweet time and now
        //            var maxage = new TimeSpan(1, 0, 0); // max age of tweets in the collection in one hour
        //            if (difference > maxage)
        //            {
        //                // remove this tweet from the collection
        //                tweets.Remove(tweet);
        //            }
        //        });
        //}
    }
}