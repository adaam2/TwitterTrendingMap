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
using FinalUniProject.helperClasses;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;

namespace FinalUniProject.TwitterLogic
{
    public static class TweetParser
    {
        // Load the 3 class distsim NLP model
        public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");
        // Instantiate the static named entity collection - persists through all instances of this class
        public static List<Entity<Tweet>> namedEntityCollection = new List<Entity<Tweet>>();
        public static IHubConnectionContext client = GlobalHost.ConnectionManager.GetHubContext<TrendsAnalysisHub>().Clients;
        public static readonly TimeSpan maxAge = new TimeSpan(0,10,0); // 10 minutes is the max age of tweets allowed in a named entity before it gets deleted
        public static readonly int thresholdNumber = 2; //number of matching tweets needed to broadcast "trend" to the hub

        // Constructor
        //public TweetParser(Tweet tweet)
        //{
        //    // Start Timer instance to run every 1 minute to remove NamedEntity instances whose last tweet is more than a 10 minutes old.
        //    //var timer = new System.Threading.Timer(e => RemoveOldTweets(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
        //    // Process the current tweet
        //    ProcessTweet(tweet);
        //}
        public static void ProcessTweet(Tweet tweet)
        {
            // Create collection instance to hold named entities detected for the current tweet
            List<Entity<Tweet>> entities = getTweetEntities(tweet);

            // If the static namedEntityCollection has a count of less than 1, then assign the current (first) tweet to this collection
            if (namedEntityCollection.Count < 1)
            {
                namedEntityCollection = entities;
            }
            else
            {
                namedEntityCollection.AddRange(entities);
            }
            List<Entity<Tweet>> joined = NamedEntityExtensions.Join(namedEntityCollection, entities).ToList<Entity<Tweet>>();
            joined.ForEach(item =>
            {
                if (item.tweets.Count > thresholdNumber)
                {
                    BroadcastToHub(item);
                }
            });
        }
        private static List<Entity<Tweet>> getTweetEntities(Tweet tweet)
        {
            var classified = _classifier.classify(tweet.Text);
            List<Entity<Tweet>> entitiesForThisTweet = new List<Entity<Tweet>>(); 

            // Establish obj reference to the various Annotation classes to pass in later to CoreLabel.get(key)
            CoreAnnotations.AnswerAnnotation ann = new CoreAnnotations.AnswerAnnotation();
            CoreAnnotations.ValueAnnotation valueAnn = new CoreAnnotations.ValueAnnotation();
            Dictionary<string, Dictionary<string, int>> entities = new Dictionary<string, Dictionary<string, int>>();

            List<ArrayList> list = CollectionExtensions.ToList<ArrayList>(classified);

            // Get background symbol - this is usually expressed as a string as "O"
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
                    // If the class name is a valid class name, then create an instance of the Named Entity sub class, using the Activator
                    Entity<Tweet> entity = NamedEntityExtensions.createNewNamedEntity(className, tweet, value);
                    entitiesForThisTweet.Add(entity);
                    }
                }
            } 
            // Return the List<T> to the caller
            return entitiesForThisTweet;        
        }     
        private static void BroadcastLog(string message)
        {
            client.All.broadcastLog(message);
        }
        private static void BroadcastToHub(Entity<Tweet> entity)
        {
            client.All.broadcastTrend(entity);
        }
        public static void RemoveOldEntities()
        {
            // This little LINQ expression removes entities whose latest updated tweet is more than the threshold max age value (for example.. latest tweet added to the "Rihanna" entity is more than an hour ago, therefore remove that entity)
            namedEntityCollection.RemoveAll(entity => DateTime.Now.Subtract(entity.tweets.Max(t => t.CreatedAt)) >= maxAge);
        }
    }
}