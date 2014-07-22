using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FinalUniProject.NERModels;
using FinalUniProject.Models;

namespace FinalUniProject.helperClasses
{
    public static class NamedEntityExtensions
    {
        public static IEnumerable<Entity<Tweet>> Join(IEnumerable<Entity<Tweet>> list1, IEnumerable<Entity<Tweet>> list2)
        {
            return list1.Join(list2, item => item.Name, item => item.Name, (outer, inner) =>
            {
                outer.tweets.AddRange(inner.tweets);
                return outer;
            });
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
    }
}