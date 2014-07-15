using System;
using System.Collections.Generic;
using FinalUniProject.Models;
namespace FinalUniProject.NERModels
{
    public abstract class NamedEntity<T> where T : TweetModel
    {
        /// <summary>
        /// Abstract modelling class for NER tagging - overridden by specific named entities. Used here so that all classes inherit from a single base class - polymorphic list
        /// </summary>
        protected string _name;
        protected Guid _uniqueID;
        protected bool _broadcast;

        public abstract Guid UniqueID { get; }
        public abstract string Name { get; set; }
        public abstract List<TweetModel> tweets { get; set; }
        public abstract bool isBroadcast { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}