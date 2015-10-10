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
    public class AuthInfo : ILatLon
    {
        public string Id { get { return GetPropFromTokenSafely<string>("sub", IdentityToken); } }
        public string Username { get { return GetPropFromTokenSafely<string>("preferred_username", IdentityToken); } }
        public string Name { get { return GetPropFromTokenSafely<string>("name", IdentityToken); } }
        public string Picture { get { return GetPropFromTokenSafely<string>("picture", IdentityToken); } }
		public string Address { get { return GetPropFromTokenSafely<string>("address", IdentityToken); } }
		public DateTimeOffset NotBefore { get { return GetPropFromTokenSafely<long> ("nbf", AccessToken).ToDateTimeOffsetFromEpoch(); } }
		public DateTimeOffset ExpiresIn { get { return GetPropFromTokenSafely<long> ("exp", AccessToken).ToDateTimeOffsetFromEpoch(); } }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string AccessToken { get; set; }
        public string IdentityToken { get; set; }
        public string RefreshToken { get; set; }

        public AuthInfo()
        {
        }

        //public void Update(AuthorizeResponse response)
        //{
        //    UpdatedAt = DateTime.UtcNow;
        //    ExpiresIn = response.ExpiresIn;
        //    IdentityToken = response.IdentityToken;
        //    AccessToken = response.AccessToken;
        //}

        public void Update(TokenResponse response)
        {
			if (IdentityToken != null) {
				IdentityToken = response.IdentityToken;
			}
            AccessToken = response.AccessToken;
            RefreshToken = response.RefreshToken;
        }

        public bool IsExpired()
        {
			if (AccessToken != null)
            {
				return DateTimeOffset.UtcNow >= ExpiresIn;
            }
            return false;
        }

        private T GetPropFromTokenSafely<T>(string prop, string token)
        {
            if (token != null)
            {
                try
                {
                    var tk = ParseToken(token);
                    return tk[prop].Value<T>();
                }
                catch (Exception ex)
                {
                    Log.Warn("AuthInfo", "Could not get prop from token: " + ex.Message);
                }
            }
            return default(T);
        }

        private JObject ParseToken(string token)
        {
            var parts = token.Split('.');
            var partAsBytes = Base64Url.Decode(parts[1]);
            var part = Encoding.UTF8.GetString(partAsBytes, 0, partAsBytes.Count());

            var jwt = JObject.Parse(part);
            return jwt;
        }

    }
}