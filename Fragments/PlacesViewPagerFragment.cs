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
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Net;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using PartyAllNight.Adapters;
using Android.Graphics;
using Android.Gms.Location.Places;
using PartyAllNight.Services;
using Android.Gms.Maps.Model;

namespace PartyAllNight.Fragments
{
    class ViewPagerFragment : Android.Support.V4.App.Fragment
    {
        private GoogleApiService _googleApiService;

        private List<List<GoogleResult>> _allPlaces;
        private LatLng _currentLocation;

        private Android.Support.V7.Widget.Toolbar _toolbar;
        private TabLayout _tabLayout;
        private ViewPager _viewPager;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            _allPlaces = new List<List<GoogleResult>>();
            _googleApiService = new GoogleApiService();
            _currentLocation = (Activity as MainActivity).GetCurrentLocation();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Fragment_PlacesViewPager, container, false);
        }

        public async override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            InitViewReferences(view);

            var radius = Arguments.GetInt(Common.Constants.BundleKeys.Radius);

            var overlay = new LoadingOverlay(Activity);
            overlay.Show();

            var pubPlaces = await _googleApiService.GetNearByPlaces(_currentLocation, radius, null, Common.Constants.LocationKeys.Pub);
            var cafePlaces = await _googleApiService.GetNearByPlaces(_currentLocation, radius,
                Common.Constants.LocationKeys.Cafe, Common.Constants.LocationKeys.Cafe);
            var nightClubPlaces = await _googleApiService.GetNearByPlaces(_currentLocation, radius,
                Common.Constants.LocationKeys.NightClub, Common.Constants.LocationKeys.NightClub);
            _allPlaces.Add(pubPlaces);
            _allPlaces.Add(cafePlaces);
            _allPlaces.Add(nightClubPlaces);
            overlay.Hide();

            InitViewPager();
        }

        private void InitViewPager()
        {
            if (Context != null)
            {
                _tabLayout.SetupWithViewPager(_viewPager);
                var adapter = new ViewPagerAdapter(Context, ChildFragmentManager);
                _viewPager.Adapter = adapter;
            }
        }

        private void InitViewReferences(View view)
        {
            _toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.Fragment_PlacesViewPager_ActionBarToolbar);
            _tabLayout = view.FindViewById<TabLayout>(Resource.Id.Fragment_PlacesViewPager_TabLayout);
            _viewPager = view.FindViewById<ViewPager>(Resource.Id.Fragment_PlacesViewPager_ViewPager);
        }

        public List<GoogleResult> GetPlaces(int index)
        {
            if (index >= _allPlaces.Count())
            {
                return null;
            }
            else
            {
                return _allPlaces[index];
            }
        }
    }
}