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
			get { return AccountStore.Create (_context).FindAccountsForService (ServiceId).Any (
                x => !string.IsNullOrWhiteSpace(x.Username)); }
		}

        public Account Get()
        {
            return AccountStore.Create(_context).FindAccountsForService(ServiceId).FirstOrDefault();
        }

        public void Save(Account account)
        {
            AccountStore.Create(_context).Save(account, ServiceId);
        }

        public Loc GetUserLoc()
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

        public void SaveUserLoc(Loc loc)
        {
            var usr = Get() ?? new Account();
            usr.Properties["lat"] = Convert.ToString(loc.Latitude);
            usr.Properties["lon"] = Convert.ToString(loc.Longitude);
            Save(usr);
        }
    }
}