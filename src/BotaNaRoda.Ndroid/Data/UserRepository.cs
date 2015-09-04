using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ModernHttpClient;
using Newtonsoft.Json;
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
			get { return AccountStore.Create (_context)
                    .FindAccountsForService (ServiceId)
                    .Any (x => !string.IsNullOrWhiteSpace(x.Username)); }
		}

        public UserInfo Get()
        {
            var user = new UserInfo();
            var acc = AccountStore.Create(_context).FindAccountsForService(ServiceId).FirstOrDefault();
            if (acc != null)
            {
                string accessToken;
                string lat;
                string lon;
                acc.Properties.TryGetValue("access_token", out accessToken);
                acc.Properties.TryGetValue("lat", out lat);
                acc.Properties.TryGetValue("lon", out lon);

                user.Username = acc.Username ?? string.Empty;
                user.AccessToken = accessToken ?? string.Empty;
                user.Latitude = Convert.ToDouble(lat, CultureInfo.InvariantCulture);
                user.Longitude = Convert.ToDouble(lon, CultureInfo.InvariantCulture);
            }
            return user;
        }

        public void Save(UserInfo userInfo)
        {
            Account acc = new Account();
            acc.Username = userInfo.Username ?? string.Empty;
            acc.Properties["access_token"] = userInfo.AccessToken ?? string.Empty;
            acc.Properties["lat"] = userInfo.Latitude.ToString(CultureInfo.InvariantCulture);
            acc.Properties["lon"] = userInfo.Longitude.ToString(CultureInfo.InvariantCulture);
            Save(acc);
        }

        public void Save(Account account)
        {
            AccountStore.Create(_context).Save(account, ServiceId);
        }
    }
}