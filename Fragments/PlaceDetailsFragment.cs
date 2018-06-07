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
using Android.Gms.Location.Places;
using PartyAllNight.Services;
using Android.Gms.Maps.Model;
using Android.Content.PM;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Square.Picasso;
using Android.Util;
using Android.Locations;
using System.Threading.Tasks;
using PartyAllNight.Common.Location;

namespace PartyAllNight.Fragments
{

    class PlaceDetailsFragment : Android.Support.V4.App.Fragment
    {
        private const int MetresInKm = 1000;
        private const int HoursInDay = 24;

        private GoogleApiService _googleApiService;

        private ImageView _mapButton;
        private ImageView _uberButton;
        private Android.Support.V7.Widget.Toolbar _toolbar;
        private FloatingActionButton _callFAB;
        private ImageView _photoImageView;
        private TextView _adressTextView;
        private TextView _openHoursTextView;
        private TextView _distanceTextView;
        private CoordinatorLayout _rootLayout;

        private GooglePlaceDetailsModel _placeDetails;
        private string _phoneNumber;
        private Location _placeLocation;
        private LatLng _currentLocation;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            _googleApiService = new GoogleApiService();
            _currentLocation = (Activity as MainActivity).GetCurrentLocation();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Fragment_PlaceDetails, container, false);
        }

        public override async void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _mapButton = view.FindViewById<ImageView>(Resource.Id.Fragment_PlaceDetails_MapSearchButton);
            _uberButton = view.FindViewById<ImageView>(Resource.Id.Fragment_PlaceDetails_UberButton);
            _toolbar = view.FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.Fragment_PlaceDetails_ActionBarToolbar);
            _callFAB = view.FindViewById<FloatingActionButton>(Resource.Id.Fragment_PlaceDetails_CallFAB);
            _photoImageView = view.FindViewById<ImageView>(Resource.Id.Fragment_PlaceDetails_PhotoImageView);
            _adressTextView = view.FindViewById<TextView>(Resource.Id.Fragment_PlaceDetails_AdresTextView);
            _distanceTextView = view.FindViewById<TextView>(Resource.Id.Fragment_PlaceDetails_DistanceTextView);
            _rootLayout = view.FindViewById<CoordinatorLayout>(Resource.Id.Fragment_PlaceDetails_Root);
            _openHoursTextView = view.FindViewById<TextView>(Resource.Id.Fragment_PlaceDetails_OpenTextView);

            _rootLayout.Visibility = ViewStates.Invisible;
            var overlay = new LoadingOverlay(Activity);
            overlay.Show();

            _mapButton.Click += OnMapButtonClick;
            _uberButton.Click += OnUberButtonClick;
            _callFAB.Click += OnCallFABClick;

            await InitPage();

            InitActionBar();

            overlay.Hide();

            _rootLayout.Visibility = ViewStates.Visible;
        }

        private async Task InitPage()
        {
            string placeId = Arguments.GetString(Common.Constants.BundleKeys.Placeid);

            try
            {
                _placeDetails = await _googleApiService.GetPlaceById(placeId);

                DisplayMetrics displayMetrics = new DisplayMetrics();
                Activity.WindowManager.DefaultDisplay.GetMetrics(displayMetrics);
                int width = displayMetrics.WidthPixels;
                var bitmap = await _googleApiService.GetPhoto(_placeDetails.result.photos.FirstOrDefault().photo_reference, width);
                _photoImageView.SetImageBitmap(bitmap);
                var span = GetOpenHours(_placeDetails);
                _openHoursTextView.Text = string.Format(GetString(Resource.String.OpenUntillFormat), span.ToString(GetString(Resource.String.TimeFormat)));
                _phoneNumber = _placeDetails.result.international_phone_number;
                _placeLocation = _placeDetails.result.geometry.location;
                _adressTextView.Text = _placeDetails.result.vicinity;
                _distanceTextView.Text = string.Format(GetString(Resource.String.DistanceFormat),
                    LocationService.GetDistance(_currentLocation, _placeDetails.result.geometry.location) / MetresInKm);
            }
            catch
            {
                if (Context != null)
                {
                    Toast.MakeText(Context, Resource.String.Error_PlaceDetails, ToastLength.Long).Show();
                }
            }
        }

        private DateTime GetOpenHours(GooglePlaceDetailsModel place)
        {
            var closeString = place.result.opening_hours.periods[(int)DateTime.Now.DayOfWeek - 1]?.close?.time;
            if (closeString != null)
            {
                var hour = Int32.Parse(closeString.Substring(0, 2));
                var minute = Int32.Parse(closeString.Substring(2, 2));
                var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
                var result = date - DateTime.Now;
                if (hour < DateTime.Now.Hour)
                {
                    result = result.Add(new TimeSpan(HoursInDay, 0, 0));
                }
                return date;
            }
            return new DateTime();
        }

        private void OnCallFABClick(object sender, EventArgs e)
        {
            Intent intent = new Intent(Intent.ActionDial);
            intent.SetData(Android.Net.Uri.Parse("tel:" + _phoneNumber));
            StartActivity(intent);
        }

        private void InitActionBar()
        {
            if (Activity != null)
            {
                var appCompatActivity = Activity as AppCompatActivity;
                appCompatActivity?.SetSupportActionBar(_toolbar);
                appCompatActivity.SupportActionBar.HideOnContentScrollEnabled = false;
                appCompatActivity.SupportActionBar.Title = _placeDetails.result.name;
            }
        }

        private void OnUberButtonClick(object sender, EventArgs e)
        {
            var uri = "http://maps.google.com/maps?&saddr=" + LocationService.CorrectLocationFormat(_currentLocation) +
                "&daddr=" + LocationService.CorrectLocationFormat(_placeLocation);

            var intent = new Intent(Intent.ActionView, Android.Net.Uri.Parse(uri));
            intent.SetPackage("com.ubercab");

            if (IsPackageAvailable("com.ubercab"))
            {
                StartActivity(intent);
            }
            else
            {
                var marketIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("market://details?id=com.ubercab"));
                marketIntent.SetPackage("com.android.vending");
                try
                {
                    StartActivity(marketIntent);
                }
                catch (ActivityNotFoundException)
                {
                    Toast.MakeText(Activity, GetString(Resource.String.Error_NoPlayStore), ToastLength.Long).Show();
                }
            }
        }

        private bool IsPackageAvailable(string package)
        {
            try
            {
                var info = (Activity.PackageManager.GetApplicationInfo(package, 0));
                return true;
            }
            catch (PackageManager.NameNotFoundException)
            {
                return false;
            }
        }

        private void OnMapButtonClick(object sender, EventArgs e)
        {
            var uri = "http://maps.google.com/maps?&saddr=" + LocationService.CorrectLocationFormat(_currentLocation) +
                "&daddr=" + LocationService.CorrectLocationFormat(_placeLocation);
            var geoUri = Android.Net.Uri.Parse(uri);
            var intent = new Intent(Intent.ActionView, geoUri);
            intent.SetPackage("com.google.android.apps.maps");

            if (IsPackageAvailable("com.google.android.apps.maps"))
            {
                StartActivity(intent);
            }
            else
            {
                var marketIntent = new Intent(Intent.ActionView, Android.Net.Uri.Parse("com.google.android.apps.maps"));
                marketIntent.SetPackage("com.android.vending");

                if (IsPackageAvailable("com.android.vending"))
                {
                    StartActivity(marketIntent);
                }
                else
                {
                    Toast.MakeText(Activity, GetString(Resource.String.Error_NoPlayStore), ToastLength.Long);
                }
            }
        }
    }
}