using FinalUniProject.Models;
using System;
using System.Collections.Generic;
using Tweetinvi.Logic.Model;
using System.Threading;
namespace FinalUniProject
{
    public static class GeoHelper
    {
        /// <summary>
        /// This method gets the centre latitude and longitude point for a set of coordinates
        /// </summary>
        /// <param name="coordList">List of Coordinates objects</param>
        /// <returns>A coordinates object representating the centre point of the List of Coordinates</returns>
        public static Coordinates GetCentrePointFromListOfCoordinates(List<Coordinates> coordList)
        {
            // Credit: http://stackoverflow.com/a/14231286/1795862

            int total = coordList.Count;

            double X = 0;
            double Y = 0;
            double Z = 0;

            foreach (var i in coordList)
            {
                double lat = i.Latitude * Math.PI / 180;
                double lon = i.Longitude * Math.PI / 180;

                double x = Math.Cos(lat) * Math.Cos(lon);
                double y = Math.Cos(lat) * Math.Sin(lon);
                double z = Math.Sin(lat);

                X += x;
                Y += y;
                Z += z;
            }

            X = X / total;
            Y = Y / total;
            Z = Z / total;

            double Lon = Math.Atan2(Y, X);
            double Hyp = Math.Sqrt(X * X + Y * Y);
            double Lat = Math.Atan2(Z, Hyp);

            return new Coordinates(Lat * 180 / Math.PI, Lon * 180 / Math.PI);
        }
        /// <summary>
        /// Retrieve the bounding box for a given SignalR connection ID
        /// </summary>
        /// <param name="connectionId">The Connection ID to retrieve the bounding box for</param>
        /// <returns></returns>
        public static BoundingBoxPoint GetUserSetBounds(string connectionId)
        {
            return SignalRUsers.Users.Find(e => e.ConnectionId == connectionId).userBoundingBox;
        }
        /// <summary>
        /// Set the bounding box of the current SignalR connectionid
        /// </summary>
        /// <param name="points">The geographic bounding box</param>
        /// <param name="connectionId">The connection ID of a SignalR client</param>
        public static void SetUserBounds(BoundingBoxPoint points, string connectionId)
        {
            if (Thread.CurrentThread.ThreadState != ThreadState.WaitSleepJoin) { 
                SignalRUser user = SignalRUsers.Users.Find(e => e.ConnectionId == connectionId);
                user.userBoundingBox = points;
            }
        }
        /// <summary>
        /// This method is used to check that the given latitudinal and longitudinal points are inside of the SignalR client's bounding box
        /// </summary>
        /// <param name="lat">The latitude to check against the bounding box</param>
        /// <param name="lng">The longitude to check against the bounding box</param>
        /// <param name="connectionId">The SignalR connection id of the current user</param>
        /// <returns></returns>
        public static bool IsInBounds(double lat, double lng, string connectionId)
        {
           // if (lat == 0 || lng == 0) return true;
            SignalRUser user = SignalRUsers.Users.Find(e => e.ConnectionId == connectionId);
            BoundingBoxPoint userSetBounds = user.userBoundingBox;
            if (userSetBounds == null) return true;
            if ((lat <= userSetBounds.NorthWestLatitude && lat >= userSetBounds.SouthEastLatitude) && (lng >= userSetBounds.NorthWestLongitude && lng <= userSetBounds.SouthEastLongitude))
            {
                // The point is in the box
                return true;
            }
            return false;
        }
        /// <summary>
        /// This function is used for local search for top entities in the statistics pane.
        /// </summary>
        /// <param name="lat">The latitude to check against the bounding box</param>
        /// <param name="lng">The longitude to check against the bounding box</param>
        /// <param name="box">The bounding box to check the coordinates against</param>
        /// <returns></returns>
        public static bool IsInBounds(double lat, double lng, BoundingBoxPoint box)
        {
            if ((lat <= box.NorthWestLatitude && lat >= box.SouthEastLatitude) && (lng >= box.NorthWestLongitude && lng <= box.SouthEastLongitude))
            {
                // The point is in the box
                return true;
            }
            return false;
        }
    }
}