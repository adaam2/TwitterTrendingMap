using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinalUniProject.NERModels;
using FinalUniProject.Models;
using System.Diagnostics;

namespace FinalUniProject.helperClasses
{
    public static class NamedEntityExtensions
    {
        public static Entity<Tweet> createNewNamedEntity(string className)
        {
            Entity<Tweet> entity = null;
            Type t = Type.GetType("FinalUniProject.NERModels." + className);
            entity = (Entity<Tweet>)Activator.CreateInstance(t);
            return entity;
        }
        public static Entity<Tweet> createNewNamedEntity(string className, Tweet tweet, string value)
        {
            Entity<Tweet> entity = null;
            Type t = Type.GetType("FinalUniProject.NERModels." + className);
            entity = (Entity<Tweet>)Activator.CreateInstance(t);
            entity.Name = value;
            // Allow for inverted index by adding tweet to NamedEntist List<TweetModel>
            if (entity.tweets == null) entity.tweets = new List<Tweet>();
            entity.tweets.Add(tweet);
            return entity;
        }
        public static int? GetDatabaseIDForEntityType(string entityTypeName)
        {
            int? theEntityTypeID = null;
            switch (entityTypeName)
            {
                case "Person":
                    theEntityTypeID = 1;
                    break;
                case "Place":
                    theEntityTypeID = 2;
                    break;
                case "Organisation":
                    theEntityTypeID = 3;
                    break;
            }
            return theEntityTypeID;
        }
        public static string GetEntityNameFromDatabaseID(int ID)
        {
            string theName = "";
            switch (ID)
            {
                case 1:
                    theName = "Person";
                    break;
                case 2:
                    theName = "Place";
                    break;
                case 3:
                    theName = "Organisation";
                    break;
            }
            return theName;
        }
    }
}