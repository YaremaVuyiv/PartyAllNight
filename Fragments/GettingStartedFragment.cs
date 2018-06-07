using System;
using Android.OS;
using Android.Views;
using Android.Widget;
using PartyAllNight.Services;
using Android.Content;
using PartyAllNight.Common.Network;
using Android.Locations;

namespace PartyAllNight.Fragments
{
    class DistanceFilterFragment : Android.Support.V4.App.Fragment
    {
        private const int DefaultRadius = 10;

        private Button _continueButton;

        private LoadingOverlay _loadingOverlay;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            return inflater.Inflate(Resource.Layout.Fragment_GettingStarted, container, false);
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

            _continueButton = view.FindViewById<Button>(Resource.Id.Fragment_GettingStarted_ConfirmButton);

            _continueButton.Click += OnButtonClick;
        }

        private void OnDistanceFilterFragmentGpsEnabled()
        {
            GetLocationAndLoadResults();
            _loadingOverlay.Hide();
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            GetLocationAndLoadResults();
        }

        private void GetLocationAndLoadResults()
        {
            (Activity as MainActivity).OnGpsEnabled -= OnDistanceFilterFragmentGpsEnabled;

            if (NetworkAvailability.Check())
            {
                if ((Activity as MainActivity).GetCurrentLocation() != null)
                {
                    //(Activity as MainActivity).OnGpsEnabled -= OnDistanceFilterFragmentGpsEnabled;

                    var fragment = new ViewPagerFragment()
                    {
                        Arguments = new Bundle()
                    };

                    fragment.Arguments.PutInt(Common.Constants.BundleKeys.Radius, DefaultRadius);

                    Activity.SupportFragmentManager
                        .BeginTransaction()
                        .Replace(Resource.Id.Activity_MainActivity_FragmentContainer, fragment)
                        .AddToBackStack(null)
                        .Commit();
                }
                else
                {
                    LocationManager manager = (LocationManager)Activity.GetSystemService(Android.Content.Context.LocationService);

                    if (!manager.IsProviderEnabled(LocationManager.GpsProvider))
                    {
                        BuildAlertMessageNoGps();
                    }

                    (Activity as MainActivity).OnGpsEnabled += OnDistanceFilterFragmentGpsEnabled;
                }
            }
            else
            {
                Toast.MakeText(Activity, GetString(Resource.String.Error_NoNetwork), ToastLength.Long).Show();
            }
        }

        private void BuildAlertMessageNoGps()
        {
            var builder = new Android.App.AlertDialog.Builder(Activity);
            builder.SetMessage(GetString(Resource.String.GpsNotEnabled))
                    .SetCancelable(false)
                    .SetPositiveButton(GetString(Resource.String.Yes), PositiveClick)
                    .SetNegativeButton(GetString(Resource.String.No), NegativeClick);

            var alert = builder.Create();
            alert.Show();
        }

        private void NegativeClick(object sender, DialogClickEventArgs args)
        {

        }

        private void PositiveClick(object sender, DialogClickEventArgs args)
        {
            _loadingOverlay = new LoadingOverlay(Activity);
            _loadingOverlay.Show();
            StartActivity(new Intent(Android.Provider.Settings.ActionLocationSourceSettings));
        }
    }
}