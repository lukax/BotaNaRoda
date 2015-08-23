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

namespace BotaNaRoda.Ndroid.Auth
{
    public class CustomOAuth2Authenticator : OAuth2Authenticator
    {
        public CustomOAuth2Authenticator(string clientId, string scope, Uri authorizeUrl, Uri redirectUrl, GetUsernameAsyncFunc getUsernameAsync = null) : base(clientId, scope, authorizeUrl, redirectUrl, getUsernameAsync)
        {
        }

        public CustomOAuth2Authenticator(string clientId, string clientSecret, string scope, Uri authorizeUrl, Uri redirectUrl, Uri accessTokenUrl, GetUsernameAsyncFunc getUsernameAsync = null) : base(clientId, clientSecret, scope, authorizeUrl, redirectUrl, accessTokenUrl, getUsernameAsync)
        {
        }

        protected override void OnPageEncountered(Uri url, IDictionary<string, string> query, IDictionary<string, string> fragment)
        {
            //TODO find a way arround this
            query = new Dictionary<string, string>(query);
            query.Remove("state");
            base.OnPageEncountered(url, query, fragment);
        }
    }
}