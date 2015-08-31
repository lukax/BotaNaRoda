using System;
using SupportActionBarDrawerToggle = Android.Support.V7.App.ActionBarDrawerToggle;
using Android.Support.V7.App;
using Android.Support.V4.Widget;

namespace BotaNaRoda.Ndroid
{
	public class MyActionBarDrawerToggle : SupportActionBarDrawerToggle
	{
		
		public MyActionBarDrawerToggle (AppCompatActivity host, DrawerLayout drawerLayout, int openedResource, int closedResource) 
			: base(host, drawerLayout, openedResource, closedResource)
		{
		}

		public override void OnDrawerOpened (Android.Views.View drawerView)
		{	
			base.OnDrawerOpened (drawerView);
		}

		public override void OnDrawerClosed (Android.Views.View drawerView)
		{
			base.OnDrawerClosed (drawerView);			
		}

		public override void OnDrawerSlide (Android.Views.View drawerView, float slideOffset)
		{
			base.OnDrawerSlide (drawerView, slideOffset);
		}
	}
}

