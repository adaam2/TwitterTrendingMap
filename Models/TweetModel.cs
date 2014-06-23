using System;
namespace FinalUniProject.Models
{
    // This class is instantiated by the TwitterStream class. The getters and setters enable dynamic expression of member variables on the fly.
    public class TweetModel
    {
        public string User { get; set; }
        public string Text { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Length { get; set; }
        public string ImageUrl { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string RealName { get; set; }
        public string ProfileUrl { get; set; }
    }
}