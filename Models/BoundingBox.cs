using System;
using System.Text;
namespace FinalUniProject.Models
{
    /// <summary>
    /// This class aims to model the Southwesterly or Northeasterly points of a bounding box
    /// </summary>
    public class BoundingBoxPoint
    {
        public double SouthWestLongitude { get; set; }
        public double SouthWestLatitude { get; set; }
        public double NorthEastLongitude { get; set; }
        public double NorthEastLatitude { get; set; }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("SW Lat: " + SouthWestLatitude + Environment.NewLine);
            sb.Append("SW Lng: " + SouthWestLongitude + Environment.NewLine);
            sb.Append("NE Lat: " + NorthEastLatitude + Environment.NewLine);
            sb.Append("NE Lng: " + NorthEastLongitude);
            return sb.ToString();
        }
    }
}