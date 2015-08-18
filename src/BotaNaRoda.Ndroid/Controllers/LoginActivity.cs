using System;
using System.Threading.Tasks;
using Android.App;
using Android.OS;
using Android.Content.PM;
using Android.Widget;
using BotaNaRoda.Ndroid.Controllers;
using Newtonsoft.Json;
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid
{
	[Activity (Label = "LoginActivity",
		ConfigurationChanges = (ConfigChanges.Orientation | ConfigChanges.ScreenSize), ParentActivity = typeof(ItemsActivity))]
	public class LoginActivity : Activity
	{

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);
			SetContentView (Resource.Layout.Login);
            ActionBar.SetDisplayHomeAsUpEnabled(true);

		    var facebook = FindViewById<Button>(Resource.Id.loginWithFacebook);
            facebook.Click += delegate
            {
                LoginToFacebook(false);
            };

		}

        void LoginToFacebook(bool allowCancel)
        {
            var auth = new OAuth2Authenticator(
                clientId: "implicitclient",
                scope: "read",
                authorizeUrl: new Uri("http://botanarodaapi.azurewebsites.net/core/connect/authorize"),
                redirectUrl: new Uri("http://botanarodaapi.azurewebsites.net/core"));

            auth.AllowCancel = allowCancel;

            auth.Error += (sender, args) =>
            {
                Console.WriteLine(args.Message);
            };
            // If authorization succeeds or is canceled, .Completed will be fired.
            auth.Completed += (s, ee) => {
                //if (!ee.IsAuthenticated)
                //{
                //    var builder = new AlertDialog.Builder(this);
                //    builder.SetMessage("Not Authenticated");
                //    builder.SetPositiveButton("Ok", (o, e) => { });
                //    builder.Create().Show();
                //    return;
                //}


            };

            var intent = auth.GetUI(this);
            StartActivity(intent);
        }


        private static readonly TaskScheduler UIScheduler = TaskScheduler.FromCurrentSynchronizationContext();

    }
}

