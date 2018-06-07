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
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;

namespace PartyAllNight.Services
{
    public class LoadingOverlay : Dialog
    {
        private Context _context;
        private Color _color;

        public LoadingOverlay(Context context) : base(context)
        {
            _context = context;

            _color = Color.GreenYellow;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            SetCanceledOnTouchOutside(false);

            var progressBar = new ProgressBar(_context)
            {
                Indeterminate = true,
            };
            progressBar.IndeterminateDrawable.SetColorFilter(_color, PorterDuff.Mode.Multiply);

            var frameLayout = new FrameLayout(_context);
            frameLayout.AddView(progressBar);

            SetContentView(frameLayout);
        }
    }
}