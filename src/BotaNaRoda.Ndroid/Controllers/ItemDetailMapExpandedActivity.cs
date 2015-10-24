using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.OS;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using BotaNaRoda.Ndroid.Util;
using Xamarin.Auth;
using Square.Picasso;
using AlertDialog = Android.App.AlertDialog;
using System.Collections.Generic;
using Android.Content;
using Android.Locations;
using Android.Support.V4.App;
using Android.Support.V4.View;
using BotaNaRoda.Ndroid.Library;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda",
        Theme = "@style/MainTheme", 
        ParentActivity = typeof(ItemDetailActivity))]
    public class ItemDetailMapExpandedActivity : AppCompatActivity, IOnMapReadyCallback
    {
        public const string ItemIdExtra = "itemId";
        public const string ItemNameExtra = "itemName";
        private string _itemId;

        private ItemDetailViewModel _item;
        private ItemRestService _itemService;
        private string _itemName;
        private MapFragment _mapFragment;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemDetailMapExpanded);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _itemId = bundle != null ? bundle.GetString(ItemIdExtra) : Intent.GetStringExtra(ItemIdExtra);
            _itemName = bundle != null ? bundle.GetString(ItemNameExtra) : Intent.GetStringExtra(ItemNameExtra);
            Title = "Localização do Produto " + _itemName;

            _mapFragment = FragmentManager.FindFragmentById<MapFragment>(Resource.Id.itemDetailMapExpandedMapFragment);

            _itemService = new ItemRestService(new UserRepository());

            Refresh();
        }

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(ItemIdExtra, _itemId);
            outState.PutString(ItemNameExtra, _itemName);
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

        private async void Refresh()
        {
            _item = await _itemService.GetItem(_itemId);
    
            _mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            LatLng location = new LatLng(_item.Latitude, _item.Longitude);
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(location);
            builder.Zoom(15);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            googleMap.AddMarker(new MarkerOptions().SetPosition(location).SetTitle(_item.Name));
            googleMap.MoveCamera(cameraUpdate);
            googleMap.MyLocationEnabled = true;
        }
    }
}

