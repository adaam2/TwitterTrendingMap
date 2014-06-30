using System;
using System.Collections;
using FinalUniProject.Models;

namespace FinalUniProject.NERModels
{
    /// <summary>
    /// Created a custom Collections class to store collections of the custom TweetModel class, used in the matching of tweets parsed by the NER
    /// </summary>
    [Serializable]
    public class TweetCollection : CollectionBase
    {
        #region Properties
        int maxItems = 2000; // max number of items in the collection
        int maxAge = 3600; // 1 hour = 3600 seconds - this is an arbitrary value used to determine how fresh tweets must be to stay in the custom collection
        #endregion
        #region Public Methods
        public TweetModel this[int index]
        {
            get
            {
                return (TweetModel)this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        public int IndexOf(TweetModel tweet)
        {
            if (tweet != null)
            {
                return base.List.IndexOf(tweet);
            }
            return -1;
        }
        public int Add(TweetModel tweet)
        {
            if (tweet != null)
            {
                return this.List.Add(tweet);
            }
            return -1;
        }
        public void Remove(TweetModel tweet)
        {
            this.InnerList.Remove(tweet);
        }
        //public void AddRange(Customers collection)
        //{
        //    if (collection != null)
        //    {
        //        this.InnerList.AddRange(collection);
        //    }
        //}
        public void Insert(int index, TweetModel tweet)
        {
            if (index <= List.Count && tweet != null)
            {
                this.List.Insert(index, tweet);
            }
        }

        public bool Contains(TweetModel tweet)
        {
            return this.List.Contains(tweet);
        }
 
        #endregion
    }
}