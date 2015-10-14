using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.View;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Library;
using Square.Picasso;
using Fragment = Android.App.Fragment;
using Object = Java.Lang.Object;

namespace BotaNaRoda.Ndroid.Controllers
{
    public class ItemImageFragment : Android.Support.V4.App.Fragment
    {
        public const string ImageDataExtra = "imgUrl";
        private string _mImageUrl;
        private ImageView _imageView;
        private Action _onImageClick;

        public static ItemImageFragment NewInstance(string imageUrl, Action onImageClick)
        {
            var fragment = new ItemImageFragment
            {
                _onImageClick = onImageClick
            };
            var bundle = new Bundle();
            bundle.PutString(ImageDataExtra, imageUrl);
            fragment.Arguments = bundle;
            return fragment;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            _mImageUrl = Arguments != null ? Arguments.GetString(ImageDataExtra) : "";
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ItemImageDetailFragment, container, false);
            _imageView = view.FindViewById<ImageView>(Resource.Id.imageView);
            _imageView.SetScaleType(ImageView.ScaleType.CenterCrop);
            _imageView.Post(() =>
            {
                Picasso.With(Activity)
                    .Load(_mImageUrl)
                    .Into(_imageView);
            });
            _imageView.Click += (sender, args) => { _onImageClick?.Invoke(); };
            return view;
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);
        }
    }
}