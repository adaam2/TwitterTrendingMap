using System;
using System.Collections.Generic;
using FinalUniProject.NERModels;

namespace FinalUniProject.Models
{
    // This class is instantiated by the TwitterStream class. The getters and setters enable dynamic expression of member variables on the fly.
    [Serializable]
    public class Tweet
    {
        // These instance fields are set when Streaming data is sent to the hub client function 
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string ProfileUrl { get; set; }
        public string URL { get; set; }
        public int databaseID { get; set; }
        public bool isDatabase { get; set; }
        // This field is set later during Tokenization / Named Entity Recognition
        public List<Entity<Tweet>> entities = new List<Entity<Tweet>>();
    }
}