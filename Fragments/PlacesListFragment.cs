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
using Android.Support.V7.Widget;
using Newtonsoft.Json;
using Android.Gms.Maps.Model;
using Java.Util;
using Android.Gms.Location.Places;
using Android.Support.V7.App;
using PartyAllNight.Services;
using Android.Locations;
using PartyAllNight.Common.Network;
using PartyAllNight.Common.Location;

namespace PartyAllNight.Fragments
{
    class LocationsFragment : Android.Support.V4.App.Fragment
    {
        private int _viewPagerIndex;
        private LatLng _currentLocation;

        private List<GoogleResult> _places;

        private RecyclerView _recyclerView;

        public LocationsFragment(int index)
        {
            _viewPagerIndex = index;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _currentLocation = (Activity as MainActivity).GetCurrentLocation();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Fragment_PlacesList, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _places = (ParentFragment as ViewPagerFragment).GetPlaces(_viewPagerIndex) ?? new List<GoogleResult>();
            _places.Sort((p1, p2) => LocationService.GetDistance(_currentLocation, p1.geometry.location)
                .CompareTo(LocationService.GetDistance(_currentLocation, p2.geometry.location)));

            _recyclerView = view.FindViewById<RecyclerView>(Resource.Id.Fragment_PlacesList_RecyclerView);
            LinearLayoutManager linearLayoutManager = new LinearLayoutManager(Activity);
            _recyclerView.SetLayoutManager(linearLayoutManager);

            var adapter = new Adapters.PlaceAdapter(Activity, _places);
            _recyclerView.SetAdapter(adapter);
            adapter.ItemClicked += AdapterItemClicked;
        }

        private void AdapterItemClicked(object sender, GoogleResult e)
        {
            if (NetworkAvailability.Check())
            {
                var fragment = new PlaceDetailsFragment()
                {
                    Arguments = new Bundle()
                };

                fragment.Arguments.PutString(Common.Constants.BundleKeys.Placeid, e.place_id);

                Activity.SupportFragmentManager
                    .BeginTransaction()
                    .Replace(Resource.Id.Activity_MainActivity_FragmentContainer, fragment)
                    .AddToBackStack(null)
                    .Commit();
            }
            else
            {
                Toast.MakeText(Activity, GetString(Resource.String.Error_NoNetwork), ToastLength.Long).Show();
            }
        }
    }
}