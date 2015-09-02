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
using Xamarin.Auth;
using com.refractored.fab;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "Bota na Roda")]
	public class ItemsFragment : Android.Support.V4.App.Fragment, ILocationListener
    {
        private RecyclerView _itemsRecyclerView;
        private ItemsListAdapter _adapter;
        private LocationManager _locMgr;
        private SwipeRefreshLayout _refresher;
        private UserRepository _userRepository;
        private ItemRestService _itemService;
		private Location _location = new Location("");

		public override View OnCreateView (LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			var view =  inflater.Inflate (Resource.Layout.Items, container, false);
			_userRepository = new UserRepository(Activity);
			_itemService = new ItemRestService(Activity, _userRepository);

			//_refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
			//_refresher.Refreshing = true;
			//_refresher.Refresh += delegate
			//{
			//	Refresh();
			//};

			view.FindViewById<FloatingActionButton> (Resource.Id.fab).Click += NewItem;

            _itemsRecyclerView = view.FindViewById<RecyclerView> (Resource.Id.itemsGridView);
		    var itemsLoader = new ItemsLoader(_itemService);
		    var sglm = new StaggeredGridLayoutManager(2, StaggeredGridLayoutManager.Vertical);
            _itemsRecyclerView.SetLayoutManager(sglm);
			_adapter = new ItemsListAdapter(Activity, itemsLoader);
			_itemsRecyclerView.SetAdapter(_adapter);
            _itemsRecyclerView.AddOnScrollListener(new InfiniteScrollListener(_adapter, sglm, 20, UpdateDataAdapter, itemsLoader));

			//_itemsRecyclerView.Click += ItemsRecyclerViewItemClick;

			return view;
		}

		public override void OnStart ()
		{
			base.OnStart ();
			_locMgr = Activity.GetSystemService(Context.LocationService) as LocationManager;
		}

		public override void OnResume ()
		{
			base.OnResume ();
            //Refresh();

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

		void ItemsRecyclerViewItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			Intent itemDetailIntent = new Intent (Activity, typeof(ItemDetailActivity));
			itemDetailIntent.PutExtra ("itemId", e.Position);
			StartActivity (itemDetailIntent);
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
            _adapter.NotifyDataSetChanged();
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

