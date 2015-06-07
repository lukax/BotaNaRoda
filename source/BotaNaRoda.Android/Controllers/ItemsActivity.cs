using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using BotaNaRoda.Android.Entity;
using Android.Locations;
using Android.Content.PM;

namespace BotaNaRoda.Android
{
    [Activity(Label = "BotaNaRoda.Android", MainLauncher = true, Icon = "@drawable/icon",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]
	public class ItemsActivity : Activity, ILocationListener
    {
		ListView _itemsListView;
		ItemsListAdapter _adapter;
		LocationManager _locMgr;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			SetContentView(Resource.Layout.Items);

			//ItemData.Service.SaveItem (new Item {Description="item1"});
			//ItemData.Service.SaveItem (new Item {Description="item2"});
			//ItemData.Service.SaveItem (new Item {Description="item3"});
			_locMgr = GetSystemService(Context.LocationService) as LocationManager;

			_itemsListView = FindViewById<ListView> (Resource.Id.itemsListView);
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
				case Resource.Id.actionNew:
					StartActivity(typeof(ItemEditActivity));
					return true;
				case Resource.Id.actionRefresh:
					ItemData.Service.RefreshCache ();
					_adapter.NotifyDataSetChanged ();
					return true;
				default:
					return base.OnOptionsItemSelected (item);
			}
		}

		protected override void OnResume ()
		{
			base.OnResume ();
			_adapter.NotifyDataSetChanged ();

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
	}
}

