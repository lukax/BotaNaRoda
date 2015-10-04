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
using Javax.Xml.Xpath;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda")]
	public class ItemsFragment : Android.Support.V4.App.Fragment, ILocationListener
    {
        private RecyclerView _itemsRecyclerView;
        private ItemsAdapter _adapter;
        private LocationManager _locMgr;
        private SwipeRefreshLayout _refresher;
        private UserRepository _userRepository;
        private ItemRestService _itemService;
        private ItemsLoader _itemsLoader;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
			var view =  inflater.Inflate (Resource.Layout.Items, container, false);
			_userRepository = new UserRepository();
			_itemService = new ItemRestService(_userRepository);
            _itemsLoader = new ItemsLoader(_itemService, 20);

			_refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.Enabled = false;
			_refresher.Refresh += delegate
			{
				_refresher.Refreshing = true;
			    Activity.RunOnUiThread(() => UpdateDataAdapter(false));
			};

			view.FindViewById<FloatingActionButton> (Resource.Id.fab).Click += NewItem;

            _itemsRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.itemsGridView);
            _itemsRecyclerView.HasFixedSize = true;
		    var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            _itemsRecyclerView.SetLayoutManager(sglm);

            _adapter = new ItemsAdapter(Activity, _itemsLoader.Items, _userRepository.Get());
            _itemsRecyclerView.SetAdapter(_adapter);
            
            var scrollListener = new InfiniteScrollListener(_adapter, sglm, () => UpdateDataAdapter(true));
            _itemsRecyclerView.AddOnScrollListener(scrollListener);



            return view;
		}

		public override void OnResume ()
		{
			base.OnResume ();
			_locMgr = Activity.GetSystemService(Context.LocationService) as LocationManager;
			_locMgr.RequestSingleUpdate (new Criteria {
				Accuracy = Accuracy.Coarse,
				PowerRequirement = Power.High
			}, this, null);

			_refresher.Refreshing = true;
            UpdateDataAdapter(false);
        }

		public override void OnPause ()
		{
			base.OnPause ();
			_locMgr.RemoveUpdates (this);
		}

		public void OnLocationChanged (Location location)
		{
            var usr = _userRepository.Get();
            usr.Latitude = location.Latitude;
            usr.Longitude = location.Longitude;
            _userRepository.Save(usr);

            _adapter.UserLatLon = usr;
			_adapter.NotifyDataSetChanged ();

			UpdateUserLocation (location);
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

        private void UpdateDataAdapter(bool fromScroll)
        {
            if ((fromScroll && _itemsLoader.CanLoadMoreItems && !_itemsLoader.IsBusy) 
                || !fromScroll)
            {
                Log.Info("InfiniteScrollListener", "Load more items requested");
                _uiCancellation = new CancellationTokenSource();
                _itemsLoader.LoadMoreItemsAsync()
                    .ContinueWith(task =>
                    {
						Activity.RunOnUiThread(() => 
						{
							_refresher.Refreshing = false;
							_adapter.NotifyDataSetChanged();
						});
					}, _uiCancellation.Token, TaskContinuationOptions.OnlyOnRanToCompletion, _uiScheduler);
            }
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
			if (_uiCancellation != null) {
				_uiCancellation.Cancel();
			}
        }

        readonly TaskScheduler _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();
        private CancellationTokenSource _uiCancellation;
    }
}

