using Android.Content;
using Android.Support.V4.App;
using FragmentManager = Android.Support.V4.App.FragmentManager;
using Fragment = Android.Support.V4.App.Fragment;
using Java.Lang;
using PartyAllNight.Fragments;

namespace PartyAllNight.Adapters
{
    class ViewPagerAdapter : FragmentStatePagerAdapter
    {
        private const int TabsCount = 3;

        private Context _context;

        public ViewPagerAdapter(Context context, FragmentManager fragmentManager) : base(fragmentManager)
        {
            _context = context;
        }

        public override int Count => TabsCount;

        public override Fragment GetItem(int position)
        {
            switch (position)
            {
                case 0:
                    {
                        return new LocationsFragment(0);
                    }
                case 1:
                    {
                        return new LocationsFragment(1);
                    }
                case 2:
                    {
                        return new LocationsFragment(2);
                    }
            }

            throw new System.Exception("Unexpected number of tabs.");
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            switch (position)
            {
                case 0:
                    {
                        return new String(_context.GetString(Resource.String.Pubs));
                    }
                case 1:
                    {
                        return new String(_context.GetString(Resource.String.Cafes));
                    }
                case 2:
                    {
                        return new String(_context.GetString(Resource.String.NightClubs));
                    }
            }

            throw new System.Exception("Unexpected count of tabs.");
        }
    }
}