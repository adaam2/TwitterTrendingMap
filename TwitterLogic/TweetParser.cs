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
            // Classify the text
            var classified = _classifier.classify(tweet.Text);

            // Establish reference to AnswerAnnotation class to pass in later to CoreLabel.get(key)
            CoreAnnotations.AnswerAnnotation ann = new CoreAnnotations.AnswerAnnotation();


            // Convert java.util.List to C# List of ArrayList of CoreLabel
            List<ArrayList> list = CollectionExtensions.ToList<ArrayList>(classified);
            list.ForEach(item =>
            {
                var arr = item;
                // Inner loop - there may be more than one CoreLabel for each word in the sentence. i.e. a word is a PLACE and a PERSON.
                foreach (CoreLabel i in arr)
                {

                    string originalText = i.originalText();
                    object entityType = i.get(ann.getClass());

                    switch (entityType.ToString())
                    {
                        case "PERSON":
                            // create new PERSON entity and add to tweetmodel entitycollection
                            Person person = new Person();
                            person.Name = originalText;
                            this.AddToEntityCollection(tweet, person);
                            break;
                        case "LOCATION":
                            // create new LOCATION entity and add to tweetmodel entitycollection
                            Place place = new Place();
                            place.Name = originalText;
                            break;
                        case "ORGANIZATION":
                            // create new ORGANIZATION entity and ditto the above
                            Organisation org = new Organisation();
                            org.Name = originalText;
                            break;
                        //case "TIME":
                            
                        //    break;
                        //case "MONEY":
                        //    // is money necessary?
                        //    break;
                        //case "PERCENT":
                        //    // will people be using percent, if so: is this something I want to capture?
                        //    break;
                        //case "DATE":
                        //    // could be useful 4/20?
                        //    break;
                        default:
                            // do error stuff or ignore

                            break;
                    }
                }
            });
        }
        private void AddToEntityCollection(TweetModel tweet, NamedEntity entity)
        {
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
        private void BroadcastToHub(List<NamedEntity> entities)
        {
            var client = GlobalHost.ConnectionManager.GetHubContext<TrendsAnalysisHub>().Clients;
            client.All.broadcastTrend(entities);
        }
        private string CleanseTweet(string text)
        {
            // remove hashtags


            // remove @ + user


            // remove links


            // remove illegal characters

            
            // remove 

            // return cleansed text

            return text;
        }
        private void RemoveOldTweets()
        {
            tweets.ForEach(tweet =>
                {
                    var now = DateTime.Now;
                    var then = tweet.CreatedAt;
                    var difference = now.Subtract(then); // finding out the difference between the tweet time and now
                    var maxage = new TimeSpan(1, 0, 0); // max age of tweets in the collection in one hour
                    if (difference > maxage)
                    {
                        // remove this tweet from the collection
                        tweets.Remove(tweet);
                    }
                });
        }
    }
}