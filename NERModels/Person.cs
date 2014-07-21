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
                _uniqueID = Guid.NewGuid();
                _name = value;
            }
        }
        public override Guid UniqueID
        {
            get
            {
                return _uniqueID;
            }
        }
        public string entityType = "Person";
        public override List<Tweet> tweets { get; set; }
    }
}