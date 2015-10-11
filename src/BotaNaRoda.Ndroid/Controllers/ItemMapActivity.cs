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

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda")]
	public class ItemMapFragment  : Android.Support.V4.App.Fragment, ILocationListener, IOnMapReadyCallback
    {
        private static readonly TaskScheduler UiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
		private IList<ItemListViewModel> _items;

        private IMenu _menu;
        private ItemRestService _itemService;
        private UserRepository _userRepository;
        private BackgroundWorker _refreshWorker;

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view =  inflater.Inflate (Resource.Layout.Items, container, false);
            
            _userRepository = new UserRepository();
            _itemService = new ItemRestService(new UserRepository());


            Refresh();

			return view;
        }

        void Refresh()
        {
            _refreshWorker = new BackgroundWorker {WorkerSupportsCancellation = true};
            _refreshWorker.DoWork += (sender, args) =>
            {
				_items = _itemService.GetAllItems(1000, 0, 20).Result;
            };
            _refreshWorker.RunWorkerCompleted += (sender, args) =>
            {
                Activity.RunOnUiThread(UpdateUi);
            };
            _refreshWorker.RunWorkerAsync();
        }

        public void OnMapReady(GoogleMap googleMap)
        {
			UpdateMapInfo (googleMap);
		}

        private void UpdateUi()
        {
			Activity.FragmentManager.FindFragmentById<MapFragment>(Resource.Id.mapFragment).GetMapAsync(callback: this);
        }

		private void UpdateMapInfo(GoogleMap googleMap)
		{
			var authInfo = _userRepository.Get ();

			LatLng location = new LatLng(authInfo.Latitude, authInfo.Longitude);
			CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
			builder.Target(location);
			builder.Zoom(12);
			CameraPosition cameraPosition = builder.Build();
			CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

			googleMap.AddMarker(new MarkerOptions().SetPosition(location).SetTitle("Ponto de Encontro"));
			googleMap.MoveCamera(cameraUpdate);
		}

    }

}

