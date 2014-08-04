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
using System.Diagnostics;

namespace FinalUniProject.TwitterLogic
{
    public static class TweetParser
    {
        // Load the 3 class distsim NLP model
        public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");

        // Instantiate the static named entity collection - persists through all instances of this class
        public static List<Entity<Tweet>> namedEntityCollection = new List<Entity<Tweet>>();

        public static IHubConnectionContext client = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;

        public static readonly TimeSpan maxAge = new TimeSpan(0,10,0); // 10 minutes is the max age of tweets allowed in a named entity before it gets deleted

        public static readonly int thresholdNumber = 10; //number of matching tweets needed to broadcast "trend" to the hub

        // array holding names of entities that have been broadcast already - pop this array every 5 minutes to ensure entities previously mentioned can be broadcast again.
        public static List<string> broadcastedEntities = new List<string>();

        // global group name
        private static string globalStreamGroupName = "Global";

        public static void ProcessTweet(Tweet tweet)
        {
            // Get entities for the tweet coming in
            List<Entity<Tweet>> entitiesForCurrentTweet = getTweetEntities(tweet); // may be more than one entity, always one tweet per entity
            if (entitiesForCurrentTweet.Count > 0)
            {
                if (namedEntityCollection.Count < 1)
                {
                    //Debug.WriteLine("entities added for first time");
                    namedEntityCollection = entitiesForCurrentTweet;
                }
                else {
                    foreach (var entity in entitiesForCurrentTweet)
                    {
                        if (namedEntityCollection.Any(item => item.Name.Trim().ToLower() == entity.Name.Trim().ToLower())) {
                            var matched = namedEntityCollection.Find(item => item.Name.Trim().ToLower() == entity.Name.Trim().ToLower());
                            matched.tweets.AddRange(entity.tweets);
                            //Debug.WriteLine("__________________________________");
                            //Debug.WriteLine(matched.Name + "(" + matched.tweets.Count + ")");
                            //Debug.WriteLine("----------------------------------");
                            //Debug.WriteLine("Containing tweets:");
                            //Debug.WriteLine("----------------------------------");
                            //matched.tweets.ForEach(t =>
                            //{
                            //    Debug.WriteLine("[" + t.Text + "]");
                            //});
                            //Debug.WriteLine("__________________________________");

                            if (matched.tweets.Count >= thresholdNumber) {
                                if (!broadcastedEntities.Contains(matched.Name.ToString().Trim().ToLower()))
                                {
                                    broadcastedEntities.Add(matched.Name.ToString().Trim().ToLower());
                                    BroadcastToHub(matched);
                                }        
                            } 
                        }
                        else
                        {
                            namedEntityCollection.Add(entity);
                            //Debug.WriteLine("New " + entity.Name + " added");
                        }
                        
                    }
                }
            }
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
        private static void BroadcastToHub(Entity<Tweet> entity)
        {
            client.All.broadcastTrend(entity);
            //SignalRUsers.Users.ForEach(user =>
            //{
            //    Debug.WriteLine(user.ConnectionId);
            //    var usersBounds = user.userBoundingBox;
            //    if (usersBounds != null)
            //    {
            //        //Debug.WriteLine("user bounds isn't null");
            //        // first check that the average center of all of the tweets for the trend are inside the bounds of the current user
            //        if (GeoHelper.IsInBounds(entity.averageCenter.Latitude, entity.averageCenter.Longitude, user.ConnectionId))
            //        {

            //            // next check that the user hasn't stopped the stream
            //            if (user.isStreamRunning)
            //            {
            //                client.Client(user.ConnectionId).broadcastTrend(entity);
            //            }
            //        }
            //    }
            //    else
            //    {
            //        //Debug.WriteLine("user bounds not set");
            //        // check that the user hasn't stopped the stream
            //        if (user.isStreamRunning)
            //        {
            //            // no bounds set, so therefore trends are nationwide for this user
            //            client.Client(user.ConnectionId).broadcastTrend(entity);
            //        }
            //    }
            //});
        }
        public static void RemoveOldEntities()
        {
            // This little LINQ expression removes entities whose latest updated tweet is more than the threshold max age value (for example.. latest tweet added to the "Rihanna" entity is more than an hour ago, therefore remove that entity)
            namedEntityCollection.RemoveAll(entity => DateTime.Now.Subtract(entity.tweets.Max(t => t.CreatedAt)) >= maxAge);
        }
    }
}