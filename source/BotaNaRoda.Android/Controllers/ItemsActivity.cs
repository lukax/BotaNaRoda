using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace BotaNaRoda.Android
{
    [Activity(Label = "BotaNaRoda.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class ItemsActivity : Activity
    {
		ListView _itemsListView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
			SetContentView(Resource.Layout.Items);

			_itemsListView = FindViewById<ListView> (Resource.Id.itemsListView);
			_itemsListView.Adapter = new ItemsListAdapter (this);
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.ItemsMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}
    }
}

