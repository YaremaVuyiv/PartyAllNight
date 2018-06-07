using Android.App;
using Android.Gms.Maps.Model;
using Android.OS;
using System.Collections.Generic;
using Android.Support.V4.App;
using Android.Gms.Common.Apis;
using Android.Content;
using Android.Gms.Common;
using Android.Runtime;
using System;
using Android.Locations;
using System.Linq;
using Android;
using Android.Content.PM;
using Android.Support.V7.App;
using Android.Gms.Location;
using PartyAllNight.Common;
using PartyAllNight.Services;
using Android.Widget;

namespace PartyAllNight
{
    [Activity(Label = "@string/AppName", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/AppTheme")]
    public class MainActivity : AppCompatActivity,
        GoogleApiClient.IConnectionCallbacks,
        GoogleApiClient.IOnConnectionFailedListener,
        Android.Gms.Location.ILocationListener
    {
        private const int RequestLocationId = 0;
        private const int LocationRequestInterval = 1000;
        private const int LocationRequestFastestInterval = 1000;

        public event Action OnGpsEnabled;

        private GoogleApiClient _googleApiClient;
        private string _locationProvider;
        private LocationRequest _locationRequest;
        private LatLng _lastReceivedLocation;
        private LocationManager _locationManager;

        public void OnLocationChanged(Android.Locations.Location location)
        {
            if (_lastReceivedLocation == null)
            {
                _lastReceivedLocation = new LatLng(location.Latitude, location.Longitude);
                OnGpsEnabled?.Invoke();
            }
            _lastReceivedLocation = new LatLng(location.Latitude, location.Longitude);
        }

        void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
        }

        protected override void OnStart()
        {
            base.OnStart();

            _googleApiClient.Connect();
        }

        protected override void OnStop()
        {
            base.OnStop();

            _googleApiClient.Disconnect();
        }

        private void RequestLocationPermisionsIfNotGranted()
        {
            var requiredPermissions = new[]
            {
                Manifest.Permission.AccessCoarseLocation,
                Manifest.Permission.AccessFineLocation,
            };

            var permissionsToRequest = requiredPermissions
                .Select(p => new { Permission = p, IsGranted = ActivityCompat.CheckSelfPermission(this, p) == Permission.Granted })
                .Where(p => !p.IsGranted)
                .Select(p => p.Permission)
                .ToArray();

            if (permissionsToRequest.Any())
            {
                ActivityCompat.RequestPermissions(this, permissionsToRequest, 1);
            }
        }

        public async override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (grantResults.ToList().All(p => p == Permission.Granted))
            {
                await LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locationRequest, this);
            }
            else
            {
                Toast.MakeText(this, "permission not granted", ToastLength.Long).Show();
            }
        }

        private void InitLocationServices()
        {
            _googleApiClient = new GoogleApiClient.Builder(this, this, this).AddApi(LocationServices.API).Build();
            _locationRequest = LocationRequest.Create();
            _locationRequest.SetPriority(LocationRequest.PriorityHighAccuracy);
            _locationRequest.SetInterval(LocationRequestInterval);
            _locationRequest.SetFastestInterval(LocationRequestFastestInterval);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Activity_Main);

            InitLocationServices();

            RequestLocationPermisionsIfNotGranted();

            InitializeLocationManager();

            var fragment = new Fragments.DistanceFilterFragment()
            {
                Arguments = new Bundle()
            };

            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.Activity_MainActivity_FragmentContainer, fragment)
                .AddToBackStack(null)
                .Commit();
        }

        public async void OnConnected(Bundle connectionHint)
        {
            var location = LocationServices.FusedLocationApi.GetLastLocation(_googleApiClient);
            if (location != null)
            {
                _lastReceivedLocation = new LatLng(location.Latitude, location.Longitude);
            }
            if (_googleApiClient.IsConnected &&
                ActivityCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) == Permission.Granted)
            {
                await LocationServices.FusedLocationApi.RequestLocationUpdates(_googleApiClient, _locationRequest, this);
            }
        }

        public LatLng GetCurrentLocation()
        {
            if (_lastReceivedLocation == null && _locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                Toast.MakeText(this, GetString(Resource.String.Error_CannotGetLocation), ToastLength.Long).Show();
            }
            return _lastReceivedLocation;
        }

        public void OnConnectionSuspended(int cause)
        {
            Toast.MakeText(this, Resource.String.Error_GoogleLocationServiceCannotConnect, ToastLength.Long);
        }

        public void OnConnectionFailed(ConnectionResult result)
        {
            Toast.MakeText(this, Resource.String.Error_GoogleLocationServiceCannotConnect, ToastLength.Long);
        }
    }
}