using Android.Content;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace BotaNaRoda.Ndroid.Controllers
{
    public class ItemImagePagerAdapter : FragmentStatePagerAdapter
    {
        private readonly Context _context;
        private readonly string[] _imageUrls;

        public ItemImagePagerAdapter(Context context, string[] imageUrls, FragmentManager fm)
            : base(fm)
        {
            _context = context;
            _imageUrls = imageUrls;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return ItemImageDetailFragment.NewInstance(_imageUrls[position]);
        }

        public override int Count
        {
            get { return _imageUrls.Length; }
        }
    }
}