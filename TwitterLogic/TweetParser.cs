using edu.stanford.nlp.ie.crf;
using edu.stanford.nlp.ling;
using FinalUniProject.IKVM.Extensions;
using FinalUniProject.Models;
using FinalUniProject.NERModels;
using java.util;
using Microsoft.AspNet.SignalR;
using FinalUniProject.Hubs;
using System;
using System.Linq;

using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;

namespace FinalUniProject.TwitterLogic
{
    public class TweetParser
    {
        public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");
        public static StringTokenizer _tokenizer;
        public static List<NamedEntity<TweetModel>> namedEntityCollection = new List<NamedEntity<TweetModel>>(); // holds entities with tweets inside of them - i.e. inverted index of the collection above
        public static IHubConnectionContext client = GlobalHost.ConnectionManager.GetHubContext<TrendsAnalysisHub>().Clients;
        public static TimeSpan maxAge = new TimeSpan(0,10,0);
        public const int thresholdNumber = 10; //number of matching tweets needed to broadcast "trend" to the hub
        public TweetParser(TweetModel tweet)
        {
            var timer = new System.Threading.Timer(e => RemoveOldTweets(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            ProcessTweet(tweet);
        }
        private void ProcessTweet(TweetModel tweet)
        {
            List<NamedEntity<TweetModel>> entities = getTweetEntities(tweet);
            if (namedEntityCollection.Count < 1)
            {
                namedEntityCollection = entities;
            }
            else
            {
                namedEntityCollection.AddRange(entities);
            }
            List<NamedEntity<TweetModel>> joined = Join(namedEntityCollection,entities).ToList<NamedEntity<TweetModel>>();
            joined.ForEach(item =>
            {
                if (item.tweets.Count > thresholdNumber)
                {
                    BroadcastToHub(item);
                }
            });
        }
        private List<NamedEntity<TweetModel>> getTweetEntities(TweetModel tweet)
        {
            List classified = _classifier.classify(tweet.Text);
            List<NamedEntity<TweetModel>> entitiesForThisTweet = new List<NamedEntity<TweetModel>>(); 
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
                    string className = "";
                    switch (answ)
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
                    if (!String.IsNullOrEmpty(className)) { 
                    NamedEntity<TweetModel> entity = createNewNamedEntity(className, tweet, value);
                    entitiesForThisTweet.Add(entity);
                    }
                }
            }      
            return entitiesForThisTweet;        
        }
        private IEnumerable<NamedEntity<TweetModel>> Join(IEnumerable<NamedEntity<TweetModel>> list1,IEnumerable<NamedEntity<TweetModel>> list2)
        {
            return list1.Join(list2, item => item.Name, item => item.Name, (outer, inner) =>
            {
                outer.tweets.AddRange(inner.tweets);
                return outer;
            });
        }
        private NamedEntity<TweetModel> createNewNamedEntity(string className, TweetModel tweet, string value)
        {
            NamedEntity<TweetModel> entity = null;
            Type t = Type.GetType("FinalUniProject.NERModels." + className);
            entity = (NamedEntity<TweetModel>)Activator.CreateInstance(t);
            entity.Name = value;
            // Allow for inverted index by adding tweet to NamedEntist List<TweetModel>
            if (entity.tweets == null) entity.tweets = new List<TweetModel>();
            entity.tweets.Add(tweet);
            return entity;
        }
        private void BroadcastLog(string message)
        {
            client.All.broadcastLog(message);
        }
        private void BroadcastToHub(NamedEntity<TweetModel> entity)
        {
            client.All.broadcastTrend(entity);
        }
        private void RemoveOldTweets()
        {
            // This little LINQ expression removes entities whose latest updated tweet is more than the threshold max age value (for example.. latest tweet added to the "Rihanna" entity is more than an hour ago, therefore remove that entity)
            namedEntityCollection.RemoveAll(entity => DateTime.Now.Subtract(entity.tweets.Max(t => t.CreatedAt)) >= maxAge);
        }
    }
}