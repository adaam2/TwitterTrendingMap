using FinalUniProject.Models;
using System;
using System.Collections.Generic;
namespace FinalUniProject.NERModels
{
    /// <summary>
    /// Model Class for the Organization tag returned by the Named Entity Recognition classifier
    /// </summary>
    public class Organisation : Entity<Tweet>
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
        public string entityType = "Organisation";
        public override List<Tweet> tweets { get; set; }
    }
}