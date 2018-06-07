using Android.App;
using Android.Content;
using Android.Net;

namespace PartyAllNight.Common.Network
{
    public static class NetworkAvailability
    {
        public static bool Check()
        {
            var connectivityManager = Application.Context.GetSystemService(Context.ConnectivityService) as ConnectivityManager;
            NetworkInfo netInfo = connectivityManager.ActiveNetworkInfo;

            return netInfo != null && netInfo.IsConnected;
        }
    }
}