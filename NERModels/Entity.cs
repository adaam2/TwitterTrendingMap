using System;
using System.Collections.Generic;
using FinalUniProject.Models;
using Tweetinvi.Logic.Model;
using LinqToWiki;
using LinqToWiki.Generated;

namespace FinalUniProject.NERModels
{
    public abstract class Entity<T> where T : Tweet
    {
        /// <summary>
        /// Abstract modelling class for NER tagging - overridden by specific named entities. Used here so that all classes inherit from a single base class - polymorphic list
        /// </summary>
        protected string _name;
        public abstract string Name { get; set; }
        public abstract List<Tweet> tweets { get; set; }
        public Coordinates averageCenter
        {
            get
            {
                // Add each tweet's longitude and latitude into a new Coordinates obj and then into a List<T>
                List<Coordinates> theCurrentListOfTweets = new List<Coordinates>();
                tweets.ForEach(tweet =>
                {
                    theCurrentListOfTweets.Add(new Coordinates(tweet.Longitude, tweet.Latitude));
                });
                // return new Coordinates obj, having calculated the average center of all of these points
                return GeoHelper.GetCentrePointFromListOfCoordinates(theCurrentListOfTweets);
            }
        }
        //public string MiscInformation
        //{
        //    get
        //    {
        //        //var wikipedia = new Wiki("TweetBot/1.0 (http://www.wherelionsroam.co.uk,adamjamesbull@googlemail.com)", "en.wikipedia.org");
        //        //PagesSource<Page> enumerable = (PagesSource<Page>)wikipedia.CreateTitlesSource(_name).Select(p => p.revisions().Select(t => t.value));
        //        //string s = "";
        //        //using (IEnumerator<string> numer = enumerable.GetEnumerator())
        //        //{
        //        //    if (numer.MoveNext()) s = numer.Current;
        //        //}
        //        //return s;
        //    }
        //}
        public override string ToString()
        {
            return Name;
        }
    }
}