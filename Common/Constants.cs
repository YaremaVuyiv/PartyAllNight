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

namespace PartyAllNight.Common
{
    public class Constants
    {
        public class BundleKeys
        {
            public const string Radius = nameof(Radius);
            public const string Placeid = nameof(Placeid);
        }

        public class LocationKeys
        {
            public const string Pub = nameof(Pub);
            public const string Cafe = "cafe";
            public const string NightClub = "night_club";
        }
    }
}