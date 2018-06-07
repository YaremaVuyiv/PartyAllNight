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
using PartyAllNight.ViewHolders;
using Square.Picasso;
using Android.Locations;
using PartyAllNight.Services;
using Android.Gms.Maps.Model;
using PartyAllNight.Common.Location;

namespace PartyAllNight.Adapters
{
    class PlaceAdapter : RecyclerView.Adapter
    {
        private const int MetresInKm = 1000;

        private Context _context;
        private IEnumerable<GoogleResult> _items;
        private LatLng _currentLocation;

        public event EventHandler<GoogleResult> ItemClicked;

        public override int ItemCount => _items?.Count() ?? 0;

        public PlaceAdapter(Context context, IEnumerable<GoogleResult> items)
        {
            _context = context;
            _items = items;
            _currentLocation = (_context as MainActivity).GetCurrentLocation();
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            var itemView = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.View_PlaceItem, parent, false);
            return new PlaceViewHolder(itemView, ViewHolderItemClicked);
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var item = _items.ElementAt(position);
            var viewHolder = holder as PlaceViewHolder;
            
            viewHolder.PlaceNameTextView.Text = item.name;
            viewHolder.PlaceAdressTextView.Text = item.vicinity;
            viewHolder.DistanceTextView.Text = String.Format(_context.GetString(Resource.String.DistanceFormat),
                LocationService.GetDistance(_currentLocation, item.geometry.location) / MetresInKm);
            viewHolder.RatingBar.Rating = (float)item.rating;
        }

        private void ViewHolderItemClicked(object sender, int index)
        {
            var item = _items.ElementAt(index);
            ItemClicked?.Invoke(sender, item);
        }
    }
}