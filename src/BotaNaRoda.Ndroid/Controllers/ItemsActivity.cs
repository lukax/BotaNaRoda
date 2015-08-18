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
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Controllers
{
    [Activity(Label = "BotaNaRoda.Android", MainLauncher = true, Icon = "@drawable/icon",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]
	public class ItemsActivity : Activity, ILocationListener
    {
		GridView _itemsListView;
		ItemsListAdapter _adapter;
		LocationManager _locMgr;
        private SwipeRefreshLayout _refresher;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			SetContentView(Resource.Layout.Items);

			//ItemData.Service.SaveItem (new Item {Description="item1"});
			//ItemData.Service.SaveItem (new Item {Description="item2"});
			//ItemData.Service.SaveItem (new Item {Description="item3"});
			_locMgr = GetSystemService(LocationService) as LocationManager;

            _refresher = FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            _refresher.Refresh += delegate
            {
                Refresh();
            };

			FindViewById<Button> (Resource.Id.newItem).Click += NewItem;
			_itemsListView = FindViewById<GridView> (Resource.Id.itemsGridView);
			_adapter = new ItemsListAdapter (this);
			_itemsListView.Adapter = _adapter;
			_itemsListView.ItemClick += _itemsListView_ItemClick;
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.ItemsMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		public override bool OnOptionsItemSelected (IMenuItem item)
		{
			switch (item.ItemId) 
			{
				default:
					return base.OnOptionsItemSelected (item);
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
            Refresh();

			Criteria criteria = new Criteria ();
			criteria.Accuracy = Accuracy.Coarse;
			criteria.PowerRequirement = Power.NoRequirement;
			string provider = _locMgr.GetBestProvider (criteria, true);
			_locMgr.RequestLocationUpdates (provider, 20000, 100, this);
		}

		protected override void OnPause ()
		{
			base.OnPause ();
			_locMgr.RemoveUpdates (this);
		}

		void _itemsListView_ItemClick (object sender, AdapterView.ItemClickEventArgs e)
		{
			Intent itemDetailIntent = new Intent (this, typeof(ItemDetailActivity));
			itemDetailIntent.PutExtra ("itemId", e.Position);
			StartActivity (itemDetailIntent);
		}

		public void OnLocationChanged (Location location)
		{
			_adapter.CurrentLocation = location;
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
		    IEnumerable<Account> accounts = AccountStore.Create(this).FindAccountsForService("BotaNaRoda");
		    if (accounts.Any())
		    {
		        StartActivity(typeof (ItemCreateActivity));
		    }
		    else
		    {
		        StartActivity(typeof(LoginActivity));
		    }
		}

        private void Refresh()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.DoWork += (sender, args) =>
            {
                ItemData.Service.RefreshCache();
                Thread.Sleep(3000);
            };
            worker.RunWorkerCompleted += (sender, args) => {
                RunOnUiThread(() =>
                {
                    _refresher.Refreshing = false;
                    _adapter.NotifyDataSetChanged();
                });
            };
            worker.RunWorkerAsync();

        }
	}
}

