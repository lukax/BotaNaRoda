using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Locations;
using Android.OS;
using Android.Support.V4.Widget;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Support.V7.Widget;
using Android.Util;
using BotaNaRoda.Ndroid.Models;
using BotaNaRoda.Ndroid.Util;
using Xamarin.Auth;
using com.refractored.fab;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda")]
	public class ItemsFragment : Android.Support.V4.App.Fragment, ILocationListener
    {
        private RecyclerView _itemsRecyclerView;
        private ItemsAdapter _adapter;
        private LocationManager _locMgr;
        private SwipeRefreshLayout _refreshLayout;
        private UserRepository _userRepository;
        private ItemRestService _itemService;
        private ItemsLoader _itemsLoader;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var view =  inflater.Inflate (Resource.Layout.Items, container, false);

            _locMgr = Activity.GetSystemService(Context.LocationService) as LocationManager;

            _userRepository = new UserRepository();
			_itemService = new ItemRestService(_userRepository);
            _itemsLoader = new ItemsLoader(Activity, _itemService, 20);
            _itemsLoader.OnItemFetched += ItemsLoaderOnOnItemFetched;

			_refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			_refreshLayout.Enabled = false;
			_refreshLayout.Refresh += delegate
			{
				Refresh();
			};

			view.FindViewById<FloatingActionButton> (Resource.Id.fab).Click += NewItem;

            _itemsRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.itemsGridView);
            _itemsRecyclerView.HasFixedSize = false;
		    var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            _itemsRecyclerView.SetLayoutManager(sglm);

            _adapter = new ItemsAdapter(Activity, _userRepository.Get());
            _itemsRecyclerView.SetAdapter(_adapter);
            
			var scrollListener = new InfiniteScrollListener(_adapter, sglm, OnItemListLoadMoreItems, _refreshLayout);
            _itemsRecyclerView.AddOnScrollListener(scrollListener);
            
            return view;
		}

        private void ItemsLoaderOnOnItemFetched(ItemListViewModel item)
        {
            Activity.RunOnUiThread(() =>
            {
                _adapter.Items.Add(item);
                _adapter.NotifyItemInserted(_adapter.Items.Count - 1);   
            });
        }

        public override void OnResume ()
		{
			base.OnResume ();
			_locMgr.RequestSingleUpdate (new Criteria {
				Accuracy = Accuracy.Coarse,
				PowerRequirement = Power.NoRequirement
			}, this, null);

            Refresh();
        }

		public override void OnPause ()
		{
			base.OnPause ();
			_locMgr.RemoveUpdates (this);
		}

		public void OnLocationChanged (Location location)
		{
            UpdateUserLocation(location);

            var usr = _userRepository.Get();
            usr.Latitude = location.Latitude;
            usr.Longitude = location.Longitude;
            _userRepository.Save(usr);

            _adapter.UserLatLon = usr;
			_adapter.NotifyDataSetChanged ();
		}

		public void OnProviderDisabled (string provider)
		{
		}

		public void OnProviderEnabled (string provider)
		{
		}

		public void OnStatusChanged (string provider, Availability status, Bundle extras)
		{
		}

		private void NewItem(object sender, EventArgs args)
		{
		    if (_userRepository.IsLoggedIn)
		    {
		        Activity.StartActivity(typeof (ItemCreateActivity));
		    }
		    else
		    {
		        Activity.StartActivity(typeof(LoginActivity));
		    }
		}

		private async void Refresh(){
            _refreshLayout.Refreshing = await _itemsLoader.LoadMoreItemsAsync();
		    _refreshLayout.Refreshing = false;
		}
        
		private async void OnItemListLoadMoreItems()
        {
            Log.Info("InfiniteScrollListener", "Load more items requested");
            _refreshLayout.Refreshing = await _itemsLoader.LoadMoreItemsAsync();
            _refreshLayout.Refreshing = false;
        }
			
		private async void UpdateUserLocation(Location location){
			Geocoder geocdr = new Geocoder(Activity);
			var addr = (await geocdr.GetFromLocationAsync(location.Latitude, location.Longitude, 1)).FirstOrDefault();
			if (addr != null) {
				await _itemService.PostUserLocalization(new UserLocalizationBindingModel
					{
						Latitude = location.Latitude,
						Longitude = location.Longitude,
						Address = addr.Thoroughfare,
						PostalCode = addr.PostalCode,
						CountryCode = addr.CountryCode,
						Locality = addr.Locality
					});
			}
		}

        public override void OnDetach()
        {
            base.OnDetach();
        }


        public override void OnCreateOptionsMenu(IMenu menu, MenuInflater inflater)
        {
            base.OnCreateOptionsMenu(menu, inflater);
            inflater.Inflate(Resource.Menu.ItemsMenu, menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                default:
                    return base.OnOptionsItemSelected(item);
            }
        }

    }
}

