using FinalUniProject.Models;
using System.Collections.Generic;
using System;
namespace FinalUniProject.NERModels
{
    /// <summary>
    /// Model Class for the Person tag returned by the Named Entity Recognition classifier
    /// </summary>
    public class Person : Entity<Tweet>
    {
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string entityType = "Person";
        public override List<Tweet> tweets { get; set; }
    }
}