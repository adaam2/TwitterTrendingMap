using System.Linq;
using Geocoding;
using Geocoding.Google;
using System.Device.Location;

namespace FinalUniProject.NERModels
{
    /// <summary>
    /// Model Class for the Place tag returned by the Named Entity Recognition classifier
    /// </summary>
    public class Place : NamedEntity
    {
        public override string Name
        {
            get
            {
                return _name;
            }
            set
            {
                //// Create geocoder object instance
                //IGeocoder geocoder = new GoogleGeocoder();

                //// Receive response from Google and take first result returned from response
                //Address addy = (Address)geocoder.Geocode(value).First();

                //// Initialize new latitude and longitude variables from Address object
                //double lat = addy.Coordinates.Latitude;
                //double lng = addy.Coordinates.Longitude;

                //// Create new instance of GeoCoordinate class, feeding in latitude and longitude values
                //Location = new GeoCoordinate(lat, lng);

                // Finally assign _name the value
                _name = value;
            }
        }
        public override string EntityName
        {
            get
            {
                return _entityName;
            }
            set
            {
                _entityName = "Place/Location";
            }
        }
        //public GeoCoordinate Location;
    }
}