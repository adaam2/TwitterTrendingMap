using System;
using System.Collections.Generic;
using FinalUniProject.Models;
namespace FinalUniProject.NERModels
{
    public abstract class Entity<T> where T : Tweet
    {
        /// <summary>
        /// Abstract modelling class for NER tagging - overridden by specific named entities. Used here so that all classes inherit from a single base class - polymorphic list
        /// </summary>
        protected string _name;
        protected Guid _uniqueID;
        protected bool _broadcast;

        public abstract Guid UniqueID { get; }
        public abstract string Name { get; set; }
        public abstract List<Tweet> tweets { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}