using FinalUniProject.Models;
using System;
using System.Collections.Generic;
using Tweetinvi.Logic.Model;
using FinalUniProject.Models;
using System.Threading;
namespace FinalUniProject
{
    public static class GeoHelper
    {
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
        public static BoundingBoxPoint GetUserSetBounds(string connectionId)
        {
            return SignalRUsers.Users.Find(e => e.ConnectionId == connectionId).userBoundingBox;
        }
        public static void SetUserBounds(BoundingBoxPoint points, string connectionId)
        {
            if (Thread.CurrentThread.ThreadState != ThreadState.WaitSleepJoin) { 
                SignalRUser user = SignalRUsers.Users.Find(e => e.ConnectionId == connectionId);
                user.userBoundingBox = points;
            }
        }
        public static bool IsInBounds(double lat, double lng, string connectionId)
        {
            SignalRUser user = SignalRUsers.Users.Find(e => e.ConnectionId == connectionId);
            BoundingBoxPoint userSetBounds = user.userBoundingBox;
            if ((lat <= userSetBounds.NorthWestLatitude && lat >= userSetBounds.SouthEastLatitude) && (lng >= userSetBounds.NorthWestLongitude && lng <= userSetBounds.SouthEastLongitude))
            {
                // The point is in the box
                return true;
            }
            return false;
        }
    }
}