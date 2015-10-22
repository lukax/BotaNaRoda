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
using Android.Util;
using Android.Views;
using Android.Widget;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;

namespace BotaNaRoda.Ndroid.Data
{
    public class UserRepository
    {
        private readonly Context _context;
        private const string ServiceId = "BotaNaRoda";

        public UserRepository()
        {
            _context = Application.Context;
        }

        public bool IsLoggedIn => AccountStore.Create(_context).FindAccountsForService(ServiceId).Any();

        public User Get()
        {
            User user = new User();
            var acc = AccountStore.Create(_context).FindAccountsForService(ServiceId).FirstOrDefault();
            if (acc != null)
            {
                string authInfo;
                if (acc.Properties.TryGetValue("authInfo", out authInfo))
                {
                    user = JsonConvert.DeserializeObject<User>(authInfo);
                }
            }
            return user;
        }

        public void Update(TokenResponse tokenResponse, UserInfoResponse userInfoResponse)
        {
            var usr = Get();
            if (tokenResponse.IdentityToken != null)
            {
                usr.IdentityToken = tokenResponse.IdentityToken;
            }

            usr.NotBefore = GetPropFromTokenSafely<long> ("nbf", tokenResponse.AccessToken).ToDateTimeOffsetFromEpoch(); 
            usr.ExpiresIn = GetPropFromTokenSafely<long> ("exp", tokenResponse.AccessToken).ToDateTimeOffsetFromEpoch();

            usr.AccessToken = tokenResponse.AccessToken;
            usr.RefreshToken = tokenResponse.RefreshToken;

            foreach (var claim in userInfoResponse.Claims)
            {
                if (claim.Item1 == JwtClaimTypes.Id)
                    usr.Id = claim.Item2;
                if (claim.Item1 == JwtClaimTypes.Name)
                    usr.Name = claim.Item2;
                if (claim.Item1 == JwtClaimTypes.PreferredUserName)
                    usr.Username = claim.Item2;
                if (claim.Item1 == JwtClaimTypes.Address)
                    usr.Address = claim.Item2;
                if (claim.Item1 == JwtClaimTypes.Picture)
                    usr.Picture = claim.Item2;
            }

            Save(usr);
        }

        public void Save(User user)
        {
			DeleteExistingAccounts();

            Account acc = new Account(
                user.Username ?? string.Empty,
                new Dictionary<string, string>
                {
                    {"authInfo", JsonConvert.SerializeObject(user)}
                });

            AccountStore.Create(_context).Save(acc, ServiceId);
        }

        public void DeleteExistingAccounts()
        {
            foreach (var acc in AccountStore.Create(_context).FindAccountsForService(ServiceId))
            {
                AccountStore.Create(_context).Delete(acc, ServiceId);
            }
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