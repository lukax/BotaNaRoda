using System;
using Android.App;
using Android.OS;
using Android.Content.PM;

namespace BotaNaRoda.Ndroid
{
	[Activity (Label = "LoginActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize))]		
	public class LoginActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);
		}

	}
}

