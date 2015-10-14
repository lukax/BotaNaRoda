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
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private string _itemId;

        private ItemDetailViewModel _item;
        private IMenu _menu;
        private ItemRestService _itemService;
        private UserRepository _userRepository;
        private BackgroundWorker _refreshWorker;
        private string _itemName;
        private LocationManager _locMgr;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ItemDetailMapExpanded);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            _itemId = bundle != null ? bundle.GetString(ItemIdExtra) : Intent.GetStringExtra(ItemIdExtra);
            _itemName = bundle != null ? bundle.GetString(ItemNameExtra) : Intent.GetStringExtra(ItemNameExtra);
            Title = "Localização do Produto " + _itemName;

            _userRepository = new UserRepository();
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

        void Refresh()
        {
            _refreshWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _refreshWorker.DoWork += (sender, args) =>
            {
                _item = _itemService.GetItem(_itemId).Result;
            };
            _refreshWorker.RunWorkerCompleted += (sender, args) =>
            {
                RunOnUiThread(UpdateUi);
            };
            _refreshWorker.RunWorkerAsync();
        }

        private void UpdateUi()
        {
            FragmentManager.FindFragmentById<MapFragment>(Resource.Id.itemDetailMapExpandedMapFragment).GetMapAsync(callback: this);
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
            googleMap.AnimateCamera(cameraUpdate);
            googleMap.MyLocationEnabled = true;
        }

        protected override void OnDestroy()
        {
			if (_refreshWorker != null) {
				_refreshWorker.CancelAsync();
			}
            base.OnDestroy();
        }

    }
}

