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
using Hangfire;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;

namespace FinalUniProject.TwitterLogic
{
    public static class TweetParser
    {
        public enum EntityType
        {
            None = 0,
            Person = 1,
            Place = 2,
            Organisation = 3
        }
        // Load the 3 class distsim NLP model
        public static CRFClassifier _classifier = CRFClassifier.getClassifierNoExceptions(@"english.all.3class.distsim.crf.ser.gz");

        // Instantiate the static named entity collection - persists through all instances of this class
        public static List<Entity<Tweet>> namedEntityCollection = new List<Entity<Tweet>>();

        public static IHubConnectionContext client = GlobalHost.ConnectionManager.GetHubContext<GeoFeedHub>().Clients;

        public static readonly TimeSpan maxAge = new TimeSpan(0, 10, 0); // 10 minutes is the max age of tweets allowed in a named entity before it gets deleted

        public static readonly int thresholdNumber = 5;
        //public static readonly int thresholdNumber = 10; //number of matching tweets needed to broadcast "trend" to the hub

        // array holding names of entities that have been broadcast already - pop this array every 5 minutes to ensure entities previously mentioned can be broadcast again.
        public static List<string> broadcastedEntities = new List<string>();

        static TweetParser()
        {
            // setup recurring jobs
            //RecurringJob.AddOrUpdate(() => RemoveOldEntities(), Cron.Daily);
            //RecurringJob.AddOrUpdate(() => SaveTopEntities(), Cron.Daily);
            //RecurringJob.AddOrUpdate(() => DeleteOldTweetsFromDatabase(), Cron.Weekly);
            //RecurringJob.AddOrUpdate(() => ClearOutEntityMemory(),"0 0 0 1/3 * ? *");
        }
        /// <summary>
        /// This method parses each tweet for its named entities and broadcasts entities that have reached the threshold number to the client (only if they haven't been broadcast before)
        /// </summary>
        /// <param name="tweet">A valid Tweet object as provided by the Tweetinvi library</param>
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
                else
                {
                    foreach (var entity in entitiesForCurrentTweet)
                    {
                        if (namedEntityCollection.Any(item => item.Name.Trim().ToLower() == entity.Name.Trim().ToLower()))
                        {
                            var matched = namedEntityCollection.Find(item => item.Name.Trim().ToLower() == entity.Name.Trim().ToLower());
                            matched.tweets.AddRange(entity.tweets);

                            //if (matched.tweets.Count >= thresholdNumber)
                            //{
                                //if (!broadcastedEntities.Contains(matched.Name.ToString().Trim().ToLower()))
                                //{
                                    broadcastedEntities.Add(matched.Name.ToString().Trim().ToLower());
                                    client.All.broadcastTrend(matched);
                                //}
                            //}
                        }
                        else
                        {
                            namedEntityCollection.Add(entity);
                        }

                        // finally, broadcast singular entity to console

                        //client.All.receiveTrendToConsole(entity);
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
                    if (!String.IsNullOrEmpty(className))
                    {
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
        }
        public static void ClearOutEntityMemory()
        {
            namedEntityCollection.Clear();
        }
        public static void RemoveOldEntities()
        {
            // This little LINQ expression removes entities whose latest updated tweet is more than the threshold max age value (for example.. latest tweet added to the "Rihanna" entity is more than an hour ago, therefore remove that entity)
            namedEntityCollection.RemoveAll(entity => DateTime.Now.Subtract(entity.tweets.Max(t => t.CreatedAt)) >= maxAge);

            // Now do the same for the db version

            //string theDeleteEntitySQL = @"DELETE FROM dbo.Entities WHERE "
        }
        /// <summary>
        /// This method saves the top 20 entities from the current collection stored in memory and sticks them into the relevant database tables 
        /// </summary>
        [AutomaticRetry(Attempts = 0)]
        public static void SaveTopEntities()
        {

            //conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString);

            IEnumerable<Entity<Tweet>> topList = null;

            //if (namedEntityCollection.Count > 20)
            //{
                //topList = namedEntityCollection.Where(entity => entity.tweets.Count > 5);
            topList = namedEntityCollection;
            //}
            //else
            //{
            //    topList = namedEntityCollection.OrderByDescending(entity => entity.tweets.Count).Take(namedEntityCollection.Count);
            //}

            // Loop over each of the entities in the top list (usually 20 in the list)
            foreach (Entity<Tweet> entity in topList.ToList())
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["mainConnection"].ConnectionString))
                {
                    int insertedID = entity.databaseID != default(int) ? entity.databaseID : default(int);

                    conn.Open();
                    if (!entity.isDatabase)
                    {
                        //first insert into entities table
                        string theInsert = @"INSERT INTO dbo.Entities(entityName,lastUpdated,averageCenterLatitude,averageCenterLongitude, entityTypeID) OUTPUT INSERTED.ID VALUES(@name, @lastUpdated, @averageCenterLatitude, @averageCenterLongitude, @entityTypeID)";
                        
                        using (SqlCommand cmd = new SqlCommand(theInsert, conn))
                        {

                            // Add cmd parameters
                            cmd.Parameters.AddWithValue("@name", entity.Name);
                            cmd.Parameters.AddWithValue("@lastUpdated", entity.tweets.Max(t => t.CreatedAt).ToString("yyyy-MM-dd HH:mm:ss"));
                            cmd.Parameters.AddWithValue("@averageCenterLatitude", entity.averageCenter.Latitude);
                            cmd.Parameters.AddWithValue("@averageCenterLongitude", entity.averageCenter.Longitude);

                            dynamic ent = entity;

                            string entityName = ent.entityType;

                            int? entityTypeID = null;

                            switch(entityName){
                                case "Person":
                                    entityTypeID = 1;
                                    break;
                                case "Place":
                                    entityTypeID = 2;
                                    break;
                                case "Organisation":
                                    entityTypeID = 3;
                                    break;
                            }
                            cmd.Parameters.AddWithValue("@entityTypeID", entityTypeID);
                            

                            // keep record of the entity id
                            insertedID = (int)cmd.ExecuteScalar();
                            entity.databaseID = insertedID;
                            entity.isDatabase = true;
                        }
                    }
                    // Instantiate a new list of ints to hold the inserted ids of the new tweets
                    List<int> insertedTweetIDs = new List<int>();

                    // Loop over each of the tweets in the current entity

                    foreach (Tweet t in entity.tweets) { 
                        if (!t.isDatabase)
                        {
                            // If there is a new tweet added to this entity, then update the last updated time for the entity
                            string theUpdateDateTimeSQL = @"UPDATE dbo.Entities SET lastUpdated = '" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "' WHERE ID = @theID";

                            using (SqlCommand updateCmd = new SqlCommand(theUpdateDateTimeSQL, conn))
                            {
                                updateCmd.Parameters.AddWithValue("@theID", insertedID);
                            }

                            // Then do the tweet insert
                            string theTweetInsert = @"INSERT INTO dbo.Tweets(tweetEncodedText, tweetDateTime, tweetUserHandle, tweetLatitude, tweetLongitude, tweetUserProfileImageUrl) OUTPUT INSERTED.ID VALUES(@text, @date, @user, @lat, @lng, @imageUrl)";

                            using (SqlCommand cmd2 = new SqlCommand(theTweetInsert, conn))
                            {
                                // Add the command params for insert into the tweets table
                                cmd2.Parameters.AddWithValue("@text", t.Text);
                                cmd2.Parameters.AddWithValue("@date", t.CreatedAt);
                                cmd2.Parameters.AddWithValue("@user", t.User);
                                cmd2.Parameters.AddWithValue("@lat", t.Latitude);
                                cmd2.Parameters.AddWithValue("@lng", t.Longitude);
                                cmd2.Parameters.AddWithValue("@imageUrl", t.ImageUrl);

                                // This query is returning one result per iteration (the ID) so use scalar

                                int tweetID = (int)cmd2.ExecuteScalar();

                                // add this ID to the tweet object for later reference
                                t.databaseID = tweetID;
                                t.isDatabase = true;
                                // Add the id to the list of inserted tweet ids for later use
                                insertedTweetIDs.Add(tweetID);

                            }
                        }
                    }


                    // loop over tweet ids and stick them along with corresponding entity ID into the linking table
                    string theLinkingSQL = @"INSERT INTO dbo.EntityTweetLink(entityID, tweetID) VALUES(@entityID, @tweetID)";
                    foreach (int tweetID in insertedTweetIDs)
                    {
                        using (SqlCommand cmd3 = new SqlCommand(theLinkingSQL, conn))
                        {

                            // Add cmd parameters

                            cmd3.Parameters.AddWithValue("@entityID", insertedID);
                            cmd3.Parameters.AddWithValue("@tweetID", tweetID);

                            // This query isn't returning any results so use nonquery.
                            cmd3.ExecuteNonQuery();
                        }
                    }

                    // Finally close db connection once and for all
                    conn.Close();
                    conn.Dispose();
                }
            }
        }
        public static void DeleteOldTweetsFromDatabase()
        {
            // Delete tweets older than 10 days - this is run every 7 days

            string theDeleteSQL = @"DELETE FROM dbo.Tweets WHERE tweetDateTime < (GETDATE() - 10)";

            Database.Query(theDeleteSQL);
        }
        
        public static List<Entity<Tweet>> GetTopEntities(EntityType type = EntityType.None, int? numberOfEntities = null, BoundingBoxPoint box = null, bool isLastUpdatedQuery = false)
        {
            // First initialize new top entities list of T<T>
            List<Entity<Tweet>> topEntities = new List<Entity<Tweet>>();
            string theTopEntitySQL = "";
            string theTopAmount = "TOP 20";

            if (box != null)
            {
                theTopAmount = "";
            }
            if (type != EntityType.None) { 
            // Then query the Entities table in the database to get top entities
                theTopEntitySQL = @"SELECT " + theTopAmount + " e1.entityName as entityName, COUNT(dbo.Tweets.ID) as tweetCount, MAX(e1.lastUpdated) as lastUpdated, STUFF((SELECT ', ' + cast(ID as varchar(20)) from dbo.Entities e2 where e2.entityName = e1.entityName FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') as listOfIDs, MAX(e1.ID) as theID, MAX(entityTypeID) as entityTypeID from dbo.Entities e1 INNER JOIN dbo.EntityTweetLink on e1.ID = dbo.EntityTweetLink.entityID INNER JOIN dbo.Tweets on dbo.EntityTweetLink.tweetID = dbo.Tweets.ID WHERE dbo.Tweets.tweetEncodedText not like '%5SOS%' AND e1.entityTypeID = " + (int)type + " GROUP BY e1.entityName HAVING COUNT(dbo.Tweets.ID) > 5 ORDER BY COUNT(dbo.Tweets.ID) desc"; //and MAX(lastUpdated) > DATEADD(Day, -" + int.Parse(ConfigurationManager.AppSettings["maxEntityLastUpdatedDaySpan"]) + ", GETDATE())
            }
            else if (isLastUpdatedQuery)
            {
                theTopEntitySQL = @"SELECT TOP 10 e1.entityName as entityName, COUNT(dbo.Tweets.ID) as tweetCount, MAX(e1.lastUpdated) as lastUpdated, STUFF((SELECT ', ' + cast(ID as varchar(20)) from dbo.Entities e2 where e2.entityName = e1.entityName FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') as listOfIDs, MAX(e1.ID) as theID, MAX(entityTypeID) as entityTypeID from dbo.Entities e1 INNER JOIN dbo.EntityTweetLink on e1.ID = dbo.EntityTweetLink.entityID INNER JOIN dbo.Tweets on dbo.EntityTweetLink.tweetID = dbo.Tweets.ID WHERE dbo.Tweets.tweetEncodedText not like '%5SOS%' GROUP BY e1.entityName HAVING COUNT(dbo.Tweets.ID) > 5 ORDER BY lastUpdated desc";
            }
            else
            {
                theTopEntitySQL = @"SELECT " + theTopAmount + " e1.entityName as entityName, COUNT(dbo.Tweets.ID) as tweetCount, MAX(e1.lastUpdated) as lastUpdated, STUFF((SELECT ', ' + cast(ID as varchar(20)) from dbo.Entities e2 where e2.entityName = e1.entityName FOR XML PATH(''), TYPE).value('.', 'NVARCHAR(MAX)'),1,1,'') as listOfIDs, MAX(e1.ID) as theID, MAX(entityTypeID) as entityTypeID from dbo.Entities e1 INNER JOIN dbo.EntityTweetLink on e1.ID = dbo.EntityTweetLink.entityID INNER JOIN dbo.Tweets on dbo.EntityTweetLink.tweetID = dbo.Tweets.ID WHERE dbo.Tweets.tweetEncodedText not like '%5SOS%' GROUP BY e1.entityName HAVING COUNT(dbo.Tweets.ID) > 5 ORDER BY COUNT(dbo.Tweets.ID) desc"; // and MAX(lastUpdated) > DATEADD(Day, -" + int.Parse(ConfigurationManager.AppSettings["maxEntityLastUpdatedDaySpan"]) + ", GETDATE())
               
            }
            // Read the data into a datatable
            DataTable entities = Database.GetAsDataTable(theTopEntitySQL);

            // Check that the datatable count is more than 1
            if (entities.Rows.Count > 0)
            {
                // iterate over DataRows
                foreach (DataRow row in entities.Rows)
                {
                    // Check that the entity in the database has an entity type
                    if (row["entityTypeID"] != System.DBNull.Value)
                    {
                        // Get reference to type of entity
                        int theEntityTypeID = int.Parse(row["entityTypeID"].ToString());

                        dynamic entity = NamedEntityExtensions.createNewNamedEntity(NamedEntityExtensions.GetEntityNameFromDatabaseID(theEntityTypeID));

                        entity.Name = row["entityName"].ToString();
                        entity.databaseID = int.Parse(row["theID"].ToString());
                        entity.entityType = NamedEntityExtensions.GetEntityNameFromDatabaseID(theEntityTypeID);

                        if (isLastUpdatedQuery)
                        {
                            entity.LastUpdated = DateTime.Parse(row["lastUpdated"].ToString());
                        }

                        // for each entity, get list of ids from entity result set and filter on those ids 
                        string theIDs = row["listOfIDs"].ToString();

                        // select top 25 tweets for this entity

                        string theSelectByIDsSQL = @"SELECT tweetEncodedText, MAX(tweetUserHandle) as userHandle, MAX(tweetLatitude) as lat, MAX(tweetLongitude) as lng, MAX(tweetUserProfileImageUrl) as imageUrl, MAX(tweetDateTime) as [date], MAX(dbo.Tweets.ID) as theID FROM [dbo].[Tweets] inner join dbo.EntityTweetLink on dbo.Tweets.ID = dbo.EntityTweetLink.tweetID  where dbo.EntityTweetLink.entityID in (" + theIDs + ") group by dbo.Tweets.tweetEncodedText order by date desc";

                        // Feed into a datatable
                        DataTable tweets = Database.GetAsDataTable(theSelectByIDsSQL);

                        List<Tweet> tweetList = new List<Tweet>();
                        // Now iterate over tweet results for all of the matching entity IDs in the database for this entity
                        foreach (DataRow tweetRow in tweets.Rows)
                        {
                            Tweet t = new Tweet()
                            {
                                Text = tweetRow["tweetEncodedText"].ToString(),
                                CreatedAt = DateTime.Parse(tweetRow["date"].ToString()),
                                Latitude = double.Parse(tweetRow["lat"].ToString()),
                                Longitude = double.Parse(tweetRow["lng"].ToString()),
                                databaseID = int.Parse(tweetRow["theID"].ToString()),
                                ImageUrl = tweetRow["imageUrl"].ToString(),
                                User = tweetRow["userHandle"].ToString()
                            };
                            tweetList.Add(t);
                        }

                        entity.tweets = tweetList;

                        //if (numberOfTweets != null) entity.tweets = entity.tweets.AsEnumerable().Take(numberOfTweets.Value).ToList();

                        topEntities.Add(entity);
                    }
                }
            }
            // Check if bounding box points have been provided for local search
            if (box != null)
            {
                var filteredList = new List<Entity<Tweet>>();
                topEntities.ForEach(entity =>
                {
                    // check that it is in bounds
                    if (GeoHelper.IsInBounds(entity.averageCenter.Latitude, entity.averageCenter.Longitude, box))
                    {
                        filteredList.Add(entity);                        
                    }
                });
                topEntities = filteredList;
            }
            if (numberOfEntities == null)
            { 
                return topEntities;
            }
            else
            {
                return topEntities.OrderByDescending(entity => entity.tweets.Count).Take(numberOfEntities.Value).ToList();
            }
        }
    }
}