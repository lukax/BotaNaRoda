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
using Android.Support.V7.Widget;
using Android.Util;
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
        private SwipeRefreshLayout _refresher;
        private UserRepository _userRepository;
        private ItemRestService _itemService;
		private Location _location = new Location("");
        private ItemsLoader _itemsLoader;

        public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view =  inflater.Inflate (Resource.Layout.Items, container, false);
			_userRepository = new UserRepository(Activity);
			_itemService = new ItemRestService(Activity, _userRepository);
            _itemsLoader = new ItemsLoader(_itemService, 20);

			_refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			//_refresher.Refreshing = true;
			//_refresher.Refresh += delegate
			//{
			//	Refresh();
			//};

			view.FindViewById<FloatingActionButton> (Resource.Id.fab).Click += NewItem;

            _itemsRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.itemsGridView);
            _itemsRecyclerView.HasFixedSize = true;
		    var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            _itemsRecyclerView.SetLayoutManager(sglm);

			_adapter = new ItemsAdapter(Activity, _itemsLoader.Items);
            _itemsRecyclerView.SetAdapter(_adapter);
            
            var scrollListener = new InfiniteScrollListener(_adapter, sglm, UpdateDataAdapter);
            _itemsRecyclerView.AddOnScrollListener(scrollListener);

            return view;
		}

		public override void OnStart ()
		{
			base.OnStart ();
			_locMgr = Activity.GetSystemService(Context.LocationService) as LocationManager;

            UpdateDataAdapter();
        }

		public override void OnResume ()
		{
			base.OnResume ();

			string provider = _locMgr.GetBestProvider (new Criteria
				{
					Accuracy = Accuracy.Coarse,
					PowerRequirement = Power.NoRequirement
				}, true);
			_locMgr.RequestLocationUpdates (provider, 20000, 100, this);
		}

		public override void OnPause ()
		{
			base.OnPause ();
			_locMgr.RemoveUpdates (this);
		}

		public void OnLocationChanged (Location location)
		{
			_location = location;
			_adapter.CurrentLocation = location;
			_adapter.NotifyDataSetChanged ();
            _userRepository.SaveLocation(_location.Latitude, _location.Longitude);
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

        private void UpdateDataAdapter()
        {
            //_refresher.Refreshing = true;
            if (_itemsLoader.CanLoadMoreItems && !_itemsLoader.IsBusy)
            {
                Log.Info("InfiniteScrollListener", "Load more items requested");
                _itemsLoader.LoadMoreItems(() =>
                {
                    Activity.RunOnUiThread(() =>
                    {
                        //_refresher.Refreshing = false;
                        _adapter.NotifyDataSetChanged();
                    });
                });
            }
        }

        private void Refresh()
        {
			_refresher.Refreshing = true;
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                _itemService.RefreshCache();
            };
            worker.RunWorkerCompleted += (sender, args) => {
                Activity.RunOnUiThread(() =>
                {
                    _refresher.Refreshing = false;
                    _adapter.NotifyDataSetChanged();
                });
            };
            worker.RunWorkerAsync();
       }

	}
}

