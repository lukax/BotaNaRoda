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
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Data
{
    public class UserService
    {
        private readonly Context _context;
        private const string ServiceId = "BotaNaRoda";

        public UserService(Context context)
        {
            _context = context;
        }

		public bool IsLoggedIn {
			get { return AccountStore.Create (_context).FindAccountsForService (ServiceId).Any (); }
		}

        public Account GetCurrentUser()
        {
            return AccountStore.Create(_context).FindAccountsForService(ServiceId).FirstOrDefault();
        }

        public void SaveCurrentUser(Account account)
        {
            AccountStore.Create(_context).Save(account, ServiceId);
        }
    }
}