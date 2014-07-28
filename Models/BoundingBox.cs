using System;
using System.Text;
using Tweetinvi.Logic.Model;
namespace FinalUniProject.Models
{
    /// <summary>
    /// This class aims to model the Southwesterly or Northeasterly points of a bounding box
    /// </summary>
    public class BoundingBoxPoint
    {
        public double SouthEastLongitude { get; set; }
        public double SouthEastLatitude { get; set; }
        public double NorthWestLongitude { get; set; }
        public double NorthWestLatitude { get; set; }

        //public void Clear()
        //{
        //    SouthEastLongitude = SouthEastLatitude = NorthWestLatitude = NorthWestLongitude = null;
        //}
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SW Lat: " + SouthEastLatitude + Environment.NewLine);
            sb.Append("SW Lng: " + SouthEastLongitude + Environment.NewLine);
            sb.Append("NE Lat: " + NorthWestLatitude + Environment.NewLine);
            sb.Append("NE Lng: " + NorthWestLongitude);
            return sb.ToString();
        }
    }
}