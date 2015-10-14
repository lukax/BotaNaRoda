using System;
using Android.Content;
using Android.Support.V4.App;
using Android.Support.V4.View;

namespace BotaNaRoda.Ndroid.Controllers
{
    public class ItemImagePagerAdapter : FragmentStatePagerAdapter
    {
        private readonly Context _context;
        private readonly string[] _imageUrls;
        private readonly Action _onImageClick;

        public ItemImagePagerAdapter(Context context, string[] imageUrls, FragmentManager fm, Action onImageClick = null)
            : base(fm)
        {
            _context = context;
            _imageUrls = imageUrls;
            _onImageClick = onImageClick;
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return ItemImageFragment.NewInstance(_imageUrls[position], _onImageClick);
        }

        public override int Count
        {
            get { return _imageUrls.Length; }
        }
    }
}