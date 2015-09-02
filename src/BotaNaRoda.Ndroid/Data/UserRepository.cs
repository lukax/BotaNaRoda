using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Models;
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Data
{
    public class UserRepository
    {
        private readonly Context _context;
        private const string ServiceId = "BotaNaRoda";

        public UserRepository(Context context)
        {
            _context = context;
        }

		public bool IsLoggedIn {
			get { return AccountStore.Create (_context).FindAccountsForService (ServiceId).Any (); }
		}

        public Account Get()
        {
            return AccountStore.Create(_context).FindAccountsForService(ServiceId).FirstOrDefault();
        }

        public void Save(Account account)
        {
            AccountStore.Create(_context).Save(account, ServiceId);
        }

        public Loc GetLocation()
        {
            var usr = Get();

            var loc = new Loc();
            if(usr != null)
            {
                loc.Latitude = Convert.ToDouble(usr.Properties["lat"]);
                loc.Longitude = Convert.ToDouble(usr.Properties["lon"]);
            }
            return loc;
        }

        public void SaveLocation(double lat, double lon)
        {
            var usr = Get();
            if (usr != null)
            {
                usr.Properties.Add("lat", lat.ToString(CultureInfo.InvariantCulture));
                usr.Properties.Add("lon", lon.ToString(CultureInfo.InvariantCulture));
                Save(usr);
            }
        }
    }
}