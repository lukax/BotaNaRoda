using System.ComponentModel;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using System.Collections.Generic;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda")]
    public class ItemsMapFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        private IList<ItemListViewModel> _items;
        private ItemRestService _itemService;
        private UserRepository _userRepository;
        private BackgroundWorker _refreshWorker;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ItemDetailMapExpanded, container, false);

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
                var usr = _userRepository.Get();
                _items = _itemService.GetAllItemsAsync(usr.Latitude, usr.Longitude, 10000, 0, 100).Result;
            };
            _refreshWorker.RunWorkerCompleted += (sender, args) =>
            {
                Activity.RunOnUiThread(UpdateUi);
            };
            _refreshWorker.RunWorkerAsync();
        }

        private void UpdateUi()
        {
            Activity.FragmentManager.FindFragmentById<MapFragment>(Resource.Id.itemDetailMapExpandedMapFragment).GetMapAsync(callback: this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var usr = _userRepository.Get();
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();
            builder.Target(new LatLng(googleMap.MyLocation.Latitude, googleMap.MyLocation.Longitude));
            builder.Zoom(15);
            CameraPosition cameraPosition = builder.Build();
            CameraUpdate cameraUpdate = CameraUpdateFactory.NewCameraPosition(cameraPosition);

            googleMap.UiSettings.CompassEnabled = true;
            googleMap.UiSettings.ZoomControlsEnabled = true;
            googleMap.UiSettings.MyLocationButtonEnabled = true;
            foreach (var item in _items)
            {
                googleMap.AddMarker(new MarkerOptions().SetPosition(new LatLng(item.Latitude, item.Longitude)).SetTitle(item.Name));
            }
            googleMap.AnimateCamera(cameraUpdate);
            googleMap.MyLocationEnabled = true;
        }

        public override void OnDetach()
        {
            if (_refreshWorker != null)
            {
                _refreshWorker.CancelAsync();
            }
            base.OnDetach();
        }

    }
}

