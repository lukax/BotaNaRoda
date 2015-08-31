
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V7.App;
using SupportToolbar = Android.Support.V7.Widget.Toolbar;
using Android.Support.V4.Widget;
using BotaNaRoda.Ndroid.Controllers;

namespace BotaNaRoda.Ndroid
{
	[Activity (Label = "Bota na Roda", MainLauncher = true, Icon = "@drawable/icon", Theme = "@style/MyTheme")]			
	public class MainActivity : AppCompatActivity
	{
		DrawerLayout mDrawerLayout;
		ListView mLeftDrawer;
		List<string> mLeftDataSet;
		ArrayAdapter<string> mLeftAdapter;
		MyActionBarDrawerToggle mDrawerToggle;
		Fragment mContainer;

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Main);

			mDrawerLayout = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
			mLeftDrawer = FindViewById<ListView>(Resource.Id.left_drawer);
			SupportActionBar.SetDisplayHomeAsUpEnabled (true);
			SupportActionBar.SetHomeButtonEnabled(true);

			mLeftDataSet = new List<string>();
			mLeftDataSet.Add ("Left Item 1");
			mLeftDataSet.Add ("Left Item 2");
			mLeftAdapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItem1, mLeftDataSet);
			mLeftDrawer.Adapter = mLeftAdapter;

			mDrawerToggle = new MyActionBarDrawerToggle(
				this,								//Host Activity
				mDrawerLayout,						//DrawerLayout
				Resource.String.ApplicationName,	//Opened Message
				Resource.String.ApplicationName		//Closed Message
			);

			mDrawerLayout.SetDrawerListener(mDrawerToggle);
			mDrawerToggle.SyncState();

			//Container
			var tx = SupportFragmentManager.BeginTransaction();
			tx.Add (Resource.Id.container, new ItemsFragment(), "ItemsFragment");
			tx.Commit ();
		}
			
		public override bool OnOptionsItemSelected (IMenuItem item)
		{		
			switch (item.ItemId)
			{
			case Android.Resource.Id.Home:
				//The hamburger icon was clicked which means the drawer toggle will handle the event
				//all we need to do is ensure the right drawer is closed so the don't overlap
				mDrawerToggle.OnOptionsItemSelected(item);
				return true;

			//case Resource.Id.action_refresh:
				//Refresh
			//	return true;


			default:
				return base.OnOptionsItemSelected (item);
			}
		}

		public override bool OnCreateOptionsMenu (IMenu menu)
		{
			MenuInflater.Inflate (Resource.Menu.ItemsMenu, menu);
			return base.OnCreateOptionsMenu (menu);
		}

		protected override void OnPostCreate (Bundle savedInstanceState)
		{
			base.OnPostCreate (savedInstanceState);
			mDrawerToggle.SyncState();
		}

		public override void OnConfigurationChanged (Android.Content.Res.Configuration newConfig)
		{
			base.OnConfigurationChanged (newConfig);
			mDrawerToggle.OnConfigurationChanged(newConfig);
		}
	}
}

