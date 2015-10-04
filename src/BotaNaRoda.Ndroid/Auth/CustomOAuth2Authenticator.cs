using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using Xamarin.Auth;
using Xamarin.Utilities;

namespace BotaNaRoda.Ndroid.Auth
{
    public class CustomOAuth2Authenticator : WebAuthenticator
    {
        private readonly UserRepository _userRepository;
        private readonly Uri _redirectUrl;

		public CustomOAuth2Authenticator(UserRepository userRepository)
        {
            _userRepository = userRepository;
            Title = "Bota na Roda";
            AllowCancel = true;
            ShowUIErrors = false;
            _redirectUrl = new Uri(Constants.ClientRedirectUrl);
        }

        public override Task<Uri> GetInitialUrlAsync()
        {
            var state = Guid.NewGuid().ToString("N");
            var nonce = Guid.NewGuid().ToString("N");

            var request = new AuthorizeRequest(Constants.IdSvrAuthorizeEndpoint);
            var startUrl = request.CreateAuthorizeUrl(
                clientId: Constants.ClientId,
                responseType: "code",
                scope: "openid profile address https://api.botanaroda.com.br offline_access",
                redirectUri: _redirectUrl.AbsoluteUri,
                state: state,
                nonce: nonce);

            TaskCompletionSource<Uri> completionSource = new TaskCompletionSource<Uri>();
            completionSource.SetResult(new Uri(startUrl));
            return completionSource.Task;
        }

        public override void OnPageLoaded(Uri url)
        {
            if (!this.UrlMatchesRedirect(url))
                return;
            this.OnRedirectPageLoaded(url);
        }

        private bool UrlMatchesRedirect(Uri url)
        {
            if (url.Host == _redirectUrl.Host)
                return url.LocalPath == _redirectUrl.LocalPath;
            return false;
        }

        protected virtual void OnRedirectPageLoaded(Uri url)
        {
            //wtf???
            var parsedUrl = url.AbsoluteUri.Substring(0, url.AbsoluteUri.LastIndexOf("#"));

			//ProgressDialog.Show ("Login", "Carregando...");

            var authorizeResponse = new AuthorizeResponse(parsedUrl);
            if (authorizeResponse.IsError)
            {
                Log.Error("CustomOAuth2Authenticator", "Could not authorize. " + authorizeResponse.Error);
                OnError(authorizeResponse.Error);
                return;
            }

            var client = new TokenClient(
                Constants.IdSvrTokenEndpoint,
                Constants.ClientId,
                Constants.ClientSecret);

            var tokenResponse = client.RequestAuthorizationCodeAsync(authorizeResponse.Code, _redirectUrl.AbsoluteUri).Result;
            if (tokenResponse.IsError)
            {
                Log.Error("CustomOAuth2Authenticator", "Could not request authorization code. " + tokenResponse.Error);
            }
				
            OnSucceeded(new Account());
        	
			var authInfo = _userRepository.Get();
			authInfo.Update(tokenResponse);
			_userRepository.Save(authInfo);
		}


    }
}