using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Library;
using Square.Picasso;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Imagem do Produto",
        Theme = "@style/MainTheme",
        ParentActivity = typeof(ItemDetailActivity))]
    public class ItemImageDetailExpandedActivity : AppCompatActivity
    {
        public const string ImageUrlsExtra = "imgUrls";
        public const string ImagePositionExtra = "imgPos";
        public const string ImageItemName = "imgItemName";
        private IList<string> _imgUrls;
        private int _imgPos;
        private string _itemName;
        private ViewPager _viewPager;
        private CirclePageIndicator _indicator;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemImageDetailExpanded);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _imgUrls = Intent.Extras.GetStringArrayList(ImageUrlsExtra);
            _imgPos = Intent.Extras.GetInt(ImagePositionExtra);
            _itemName = Intent.Extras.GetString(ImageItemName);

            Title = "Fotos do Produto " + _itemName;

            _viewPager = FindViewById<ViewPager>(Resource.Id.itemsDetailViewPager);
            _indicator = FindViewById<CirclePageIndicator>(Resource.Id.indicator);

            _viewPager.Adapter = new ItemImagePagerAdapter(this, _imgUrls.ToArray(), SupportFragmentManager);
            _viewPager.SetCurrentItem(_imgPos, true);
            _indicator.SetViewPager(_viewPager);
            _indicator.SetSnap(true);
        }


        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Android.Resource.Id.Home:
                    NavUtils.NavigateUpFromSameTask(this);
                    return true;
            }
            return base.OnOptionsItemSelected(item);
        }
    }
}