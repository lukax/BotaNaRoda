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

namespace BotaNaRoda.Ndroid
{
    public class Constants
    {
        public const string ClientId = "android.botanaroda.com.br";
        public const string ClientSecret = "150D23DAE26C4629BC58A7104B1AECFB";
        public const string ClientRedirectUrl = "https://botanaroda.azurewebsites.net/core";

        public const string IdSvrAuthorizeEndpoint = "https://botanaroda.azurewebsites.net/core/connect/authorize";
        public const string IdSvrTokenEndpoint = "https://botanaroda.azurewebsites.net/core/connect/token";

        public const string BotaNaRodaEndpoint = "https://botanaroda.azurewebsites.net/api";
    }
}