using System.ComponentModel;
using Android.App;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Views;
using BotaNaRoda.Ndroid.Data;
using BotaNaRoda.Ndroid.Models;
using System.Collections.Generic;
using System.Linq;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda")]
    public class ItemsMapFragment : Android.Support.V4.App.Fragment, IOnMapReadyCallback
    {
        private IList<ItemListViewModel> _items;
        private ItemRestService _itemService;
        private UserRepository _userRepository;
        private MapFragment _mapFragment;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.ItemDetailMapExpanded, container, false);

            _userRepository = new UserRepository();
            _itemService = new ItemRestService(new UserRepository());

            _mapFragment = Activity.FragmentManager.FindFragmentById<MapFragment>(Resource.Id.itemDetailMapExpandedMapFragment);

            Refresh();

            return view;
        }

        private async void Refresh()
        {
            var usr = _userRepository.Get();
            _items = await _itemService.GetAllItemsAsync(usr.Latitude, usr.Longitude, 10000, 0, 100);
           
            _mapFragment.GetMapAsync(this);
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            var usr = _userRepository.Get();
            CameraPosition.Builder builder = CameraPosition.InvokeBuilder();

            double targetLat = usr.Latitude;
            double targetLon = usr.Longitude;
            if (usr.Latitude == 0 && usr.Longitude == 0 && _items.Count > 0)
            {
                targetLat = _items.First().Latitude;
                targetLon = _items.First().Longitude;
            }

            builder.Target(new LatLng(targetLat, targetLon));
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

        public override void OnDestroyView()
        {
            if (_mapFragment != null)
            {
                Activity.FragmentManager.BeginTransaction().Remove(_mapFragment).Commit();
            }
            base.OnDestroyView();
        }
    }
}

