using System.Collections.Generic;

namespace PartyAllNight
{
    public class GooglePlaceDetailsModel
    {
        public string next_page_token;
        public List<GoogleResult> results;
        public string status;
        public GoogleResult result;
        public List<OpeningHours> opening_hours;
    }

    public class GoogleResult
    {
        public GoogleGeometry geometry;
        public string icon;
        public string id;
        public string name;
        public OpeningHours opening_hours;
        public List<Photo> photos;
        public string place_id;
        public string scope;
        public int price_level;
        public double rating;
        public string reference;
        public string[] types;
        public string vicinity;
        public string international_phone_number;
    }

    public class Photo
    {
        public double height;
        public string[] html_attributes;
        public string photo_reference;
        public double width;
    }

    public class OpeningHours
    {
        public bool open_now;
        public List<Period> periods;
    }

    public class GoogleGeometry
    {
        public Location location;
    }

    public class Location
    {
        public double lat;
        public double lng;
    }

    public class Period
    {
        public Close close;
    }

    public class Close
    {
        public int day;
        public string time;
    }
}