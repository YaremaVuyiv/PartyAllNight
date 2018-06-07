using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Gms.Maps.Model;
using Android.Locations;

namespace PartyAllNight.Common.Location
{
    public static class LocationService
    {
        public static string CorrectLocationFormat(LatLng location)
        {
            return location.Latitude.ToString().Replace(',', '.') + "," + location.Longitude.ToString().Replace(',', '.');
        }

        public static string CorrectLocationFormat(PartyAllNight.Location location)
        {
            return location.lat.ToString().Replace(',', '.') + "," + location.lng.ToString().Replace(',', '.');
        }

        public static double GetDistance(LatLng startLocation, PartyAllNight.Location endLocation)
        {
            float[] results = new float[1];
            Android.Locations.Location.DistanceBetween(endLocation.lat, endLocation.lng, startLocation.Latitude, startLocation.Longitude, results);

            return results.FirstOrDefault();
        }
    }
}