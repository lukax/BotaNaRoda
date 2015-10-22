using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IdentityModel.Client;

namespace BotaNaRoda.Ndroid.Auth
{
    public class IdSvrOAuth2Util
    {
        public const string StandardScopes = "openid profile address https://api.botanaroda.com.br offline_access";

        public static async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var client = new UserInfoClient(
                new Uri(Constants.IdSvrUserInfoEndpoint),
                accessToken);

            return await client.GetAsync();
        }

        public static async Task<TokenResponse> RequestTokenForFacebookGrantAsync(string facebookAccessToken)
        {
            var client = new TokenClient(
                Constants.IdSvrTokenEndpoint,
                Constants.ClientIdCustomGrant,
                Constants.ClientSecret);

            var customParameters = new Dictionary<string, string>
                {
                    { "access_token", facebookAccessToken }
                };

            return await client.RequestCustomGrantAsync("facebook", StandardScopes, customParameters);
        }

        public static async Task<TokenResponse> ExchangeRefreshToken(string refreshToken)
        {
            var httpClient = new HttpClient(/*new NativeMessageHandler()*/);
            //bug doing this manually because of crazy issue with IdentityModel
            var httpResponseMessage = await httpClient.PostAsync(Constants.IdSvrTokenEndpoint,
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                            { "client_id", Constants.ClientId },
                            { "client_secret", Constants.ClientSecret },
                            { "grant_type", "refresh_token" },
                            { "refresh_token", refreshToken }
                }));
            var raw = await httpResponseMessage.Content.ReadAsStringAsync();

            var tokenResponse = new TokenResponse(raw);
            return tokenResponse;
        }
    }
}