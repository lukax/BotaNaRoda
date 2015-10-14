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
            SetContentView(Resource.Layout.ItemDetailImageExpanded);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _imgUrls = bundle != null ? bundle.GetStringArrayList(ImageUrlsExtra) : Intent.Extras.GetStringArrayList(ImageUrlsExtra);
            _imgPos = bundle != null ? bundle.GetInt(ImagePositionExtra) : Intent.Extras.GetInt(ImagePositionExtra);
            _itemName = bundle != null ? bundle.GetString(ImageItemName) : Intent.Extras.GetString(ImageItemName);

            Title = "Fotos do Produto " + _itemName;

            _viewPager = FindViewById<ViewPager>(Resource.Id.itemsDetailViewPager);
            _indicator = FindViewById<CirclePageIndicator>(Resource.Id.indicator);

            _viewPager.Adapter = new ItemImagePagerAdapter(this, _imgUrls.ToArray(), SupportFragmentManager);
            _viewPager.SetCurrentItem(_imgPos, true);
            _indicator.SetViewPager(_viewPager);
            _indicator.SetSnap(true);
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutStringArrayList(ImageUrlsExtra, _imgUrls);
            outState.PutInt(ImagePositionExtra, _imgPos);
            outState.PutString(ImageItemName, _itemName);
            base.OnSaveInstanceState(outState);
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