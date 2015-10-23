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
        public const string BundleItemsFilter = "BundleItemsFilter";

        private RecyclerView _itemsRecyclerView;
        private ItemsAdapter _adapter;
        private LocationManager _locMgr;
        private SwipeRefreshLayout _refreshLayout;
		private TextView _itemsEmptyText;
        private UserRepository _userRepository;
        private ItemRestService _itemService;
        private ItemsLoader _itemsLoader;
        private ItemsLoader.Filter _itemsFilter = ItemsLoader.Filter.AllItems;
		private CancellationTokenSource _uiCancellationToken;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var view =  inflater.Inflate (Resource.Layout.Items, container, false);

			_uiCancellationToken = new CancellationTokenSource ();

			var defaultFilterExtra = _itemsFilter.ToString ();
            if (savedInstanceState != null)
            {
				var filterExtra = savedInstanceState.GetString(BundleItemsFilter, _itemsFilter.ToString()) ?? defaultFilterExtra;
                Enum.TryParse(filterExtra, out _itemsFilter);
            }
			if (Arguments != null) {
				var filterExtra = Arguments.GetString(BundleItemsFilter, _itemsFilter.ToString()) ?? defaultFilterExtra;
				Enum.TryParse(filterExtra, out _itemsFilter);
			}

            _locMgr = Activity.GetSystemService(Context.LocationService) as LocationManager;

            _userRepository = new UserRepository();
			_itemService = new ItemRestService(_userRepository);
            _itemsLoader = new ItemsLoader(Activity, _userRepository, _itemService, 20, _itemsFilter);
            _itemsLoader.OnItemFetched += ItemsLoaderOnOnItemFetched;

			_refreshLayout = view.FindViewById<SwipeRefreshLayout>(Resource.Id.itemsRefreshLayout);
            _refreshLayout.Refresh += (sender, args) => Refresh();

			_itemsEmptyText = view.FindViewById<TextView> (Resource.Id.itemsEmptyText);
			view.FindViewById<FloatingActionButton> (Resource.Id.fab).Click += NewItem;

            _itemsRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.itemsGridView);
            _itemsRecyclerView.HasFixedSize = false;
		    var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            _itemsRecyclerView.SetLayoutManager(sglm);

            _adapter = new ItemsAdapter(Activity, _userRepository.Get());
            _itemsRecyclerView.SetAdapter(_adapter);
            
			var scrollListener = new InfiniteScrollListener(_adapter, sglm, OnItemListLoadMoreItems, _refreshLayout);
            _itemsRecyclerView.AddOnScrollListener(scrollListener);

			Refresh();

            return view;
		}

		public override void OnDestroyView()
		{
			base.OnDestroyView();
			_uiCancellationToken.Cancel ();
			_itemsLoader.OnItemFetched -= ItemsLoaderOnOnItemFetched;
		}

        public override void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);
            outState.PutString(BundleItemsFilter, _itemsFilter.ToString());
        }

        private void ItemsLoaderOnOnItemFetched(ItemListViewModel item)
        {
			if (Activity != null) {
				Activity.RunOnUiThread(() =>
					{
						_adapter.Items.Add(item);
						_adapter.NotifyItemInserted(_adapter.Items.Count - 1);   
					});
			}
        }

        public override void OnResume ()
		{
			base.OnResume ();
			_locMgr.RequestSingleUpdate (new Criteria {
				Accuracy = Accuracy.Coarse,
				PowerRequirement = Power.NoRequirement
			}, this, null);

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
                Intent loginIntent = new Intent(Activity, typeof(LoginActivity));
		        loginIntent.PutExtra(LoginActivity.PendingActionBundleKey, LoginActivity.PendingAction.PostItem.ToString());
		        Activity.StartActivity(loginIntent);
		    }
		}

		private void Refresh(){
			_refreshLayout.Refreshing = _itemsLoader.LoadMoreItemsAsync(_uiCancellationToken);
		    _refreshLayout.Refreshing = false;
		}
        
		private void OnItemListLoadMoreItems()
        {
            Log.Info("InfiniteScrollListener", "Load more items requested");
			_refreshLayout.Refreshing = _itemsLoader.LoadMoreItemsAsync(_uiCancellationToken);
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

