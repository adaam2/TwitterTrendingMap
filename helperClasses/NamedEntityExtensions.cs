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
        public static IList<NamedEntity<T>> MergeEntities<T>(this IList<NamedEntity<T>> list1, IList<NamedEntity<T>> list2)
where T : TweetModel
        {
            foreach (NamedEntity<T> entity1 in list1)
            {
                foreach (NamedEntity<T> entity2 in list2)
                {
                    if (entity1.Name == entity2.Name)
                    {
                        entity1.tweets.AddRange(entity2.tweets);
                    }
                }
            }

            return list1; //original list will get augmented but returning it allows chaining
        }
        public static IEnumerable<NamedEntity<TweetModel>> Join(IEnumerable<NamedEntity<TweetModel>> list1, IEnumerable<NamedEntity<TweetModel>> list2)
        {
            return list1.Join(list2, item => item.Name, item => item.Name, (outer, inner) =>
            {
                outer.tweets.AddRange(inner.tweets);
                return outer;
            });
        }
        public static NamedEntity<TweetModel> createNewNamedEntity(string className, TweetModel tweet, string value)
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
    }
}