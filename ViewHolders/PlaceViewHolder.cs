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

namespace PartyAllNight.ViewHolders
{
    class PlaceViewHolder : RecyclerView.ViewHolder
    {
        public TextView PlaceNameTextView { get; set; }
        public TextView PlaceAdressTextView { get; set; }
        public TextView DistanceTextView { get; set; }
        public RatingBar RatingBar { get; set; }

        public PlaceViewHolder(View itemView, EventHandler<int> clickHandler) : base(itemView)
        {
            PlaceNameTextView = itemView.FindViewById<TextView>(Resource.Id.View_PlaceItem_NameTextView);
            PlaceAdressTextView = itemView.FindViewById<TextView>(Resource.Id.View_PlaceItem_AdressTextView);
            DistanceTextView = itemView.FindViewById<TextView>(Resource.Id.View_PlaceItem_DistanceTextView);
            RatingBar = itemView.FindViewById<RatingBar>(Resource.Id.View_PlaceItem_RatingBar);

            itemView.Click += (sender, args) => clickHandler?.Invoke(sender, AdapterPosition);
        }
    }
}