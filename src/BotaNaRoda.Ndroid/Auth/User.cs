using System;
using System.Linq;
using System.Text;
using Android.Util;
using BotaNaRoda.Ndroid.Models;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace BotaNaRoda.Ndroid.Data
{
    public class User : ILatLon
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string Picture { get; set; }
		public string Address { get; set; }
		public DateTimeOffset NotBefore { get; set; }
		public DateTimeOffset ExpiresIn { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AccessToken { get; set; }
        public string IdentityToken { get; set; }
        public string RefreshToken { get; set; }

        public User()
        {
        }

        public bool IsExpired()
        {
			if (AccessToken != null)
            {
				return DateTimeOffset.UtcNow >= ExpiresIn;
            }
            return false;
        }
    }
}