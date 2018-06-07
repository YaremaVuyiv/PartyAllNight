using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Android.Gms.Maps.Model;
using Newtonsoft.Json;
using Android.Graphics;
using Android.Util;
using Java.Net;
using Android.Locations;
using PartyAllNight.Common.Location;
using PartyAllNight.Common.Network;

namespace PartyAllNight.Services
{
    class GoogleApiService
    {
        private const string StrGoogleApiKey = "AIzaSyDx0zFQ0KXfgWhw_CY7ja7F52hYIwf5Z2o";
        private const int MetresInKm = 1000;

        public async Task<List<GoogleResult>> GetNearByPlaces(LatLng location, int radius, string type, string keyword)
        {
            if (NetworkAvailability.Check())
            {
                try
                {
                    string json = string.Empty;
                    if (type != null)
                    {
                        json = await DownloadString("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location="
                            + LocationService.CorrectLocationFormat(location) + "&radius=" + radius * MetresInKm + "&type=" + type + "&keyword=" + keyword + "&key=" + StrGoogleApiKey);
                    }
                    else
                    {
                        json = await DownloadString("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location="
                            + LocationService.CorrectLocationFormat(location) + "&radius=" + radius * MetresInKm + "&keyword=" + keyword + "&key=" + StrGoogleApiKey);
                    }
                    var result = JsonConvert.DeserializeObject<GooglePlaceDetailsModel>(json);
                    var places = result.results.Where(p => p.opening_hours == null ? false : p.opening_hours.open_now).ToList();

                    return places;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        public async Task<GooglePlaceDetailsModel> GetPlaceById(string placeId)
        {
            if (NetworkAvailability.Check())
            {
                try
                {
                    var json = await DownloadString("https://maps.googleapis.com/maps/api/place/details/json?placeid="
                        + placeId + "&key=" + StrGoogleApiKey);



                    var result = JsonConvert.DeserializeObject<GooglePlaceDetailsModel>(json);
                    var place = result;

                    return place;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            return null;
        }

        public async Task<Bitmap> GetPhoto(string photoReference, int width)
        {
            if (NetworkAvailability.Check())
            {
                Bitmap bitmap = null;
                await Task.Run(() =>
                {
                    try
                    {
                        URL url = new URL("https://maps.googleapis.com/maps/api/place/photo?maxwidth=" + width + "&photoreference="
                            + photoReference + "&key=" + StrGoogleApiKey);
                        var connection = (HttpURLConnection)url.OpenConnection();
                        connection.Connect();
                        var stream = connection.InputStream;
                        bitmap = BitmapFactory.DecodeStream(stream);


                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                });
                return bitmap;
            }
            return null;
        }

        private async Task<string> DownloadString(string strUri)
        {
            WebClient webclient = new WebClient();
            string strResultData;
            try
            {
                strResultData = await webclient.DownloadStringTaskAsync(new Uri(strUri));
            }
            catch
            {
                strResultData = "Exception";
            }
            finally
            {
                webclient.Dispose();
                webclient = null;
            }

            return strResultData;
        }
    }
}