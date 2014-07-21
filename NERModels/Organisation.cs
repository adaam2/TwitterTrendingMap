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
                _uniqueID = Guid.NewGuid();
                _name = value;
            }
        }
        public override Guid UniqueID
        {
            get {
                return _uniqueID;
            }
        }
        public string entityType = "Organisation";
        public override List<Tweet> tweets { get; set; }
    }
}