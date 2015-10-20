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
        public const string ClientIdCustomGrant = "custom.android.botanaroda.com.br";
        public const string ClientSecret = "150D23DAE26C4629BC58A7104B1AECFB";
        public const string ClientRedirectUrl = "https://botanaroda.azurewebsites.net/core";

		//-------
        public const string IdSvrAuthorizeEndpoint = "https://botanaroda.azurewebsites.net/core/connect/authorize";
		public const string IdSvrTokenEndpoint = "https://botanaroda.azurewebsites.net/core/connect/token";
		public const string IdSvrUserInfoEndpoint = "https://botanaroda.azurewebsites.net/core/connect/userinfo";
		public const string BotaNaRodaEndpoint = "https://botanaroda.azurewebsites.net/api";
//
//		public const string IdSvrAuthorizeEndpoint = "http://192.168.1.106:44200/core/connect/authorize";
//		public const string IdSvrTokenEndpoint = "http://192.168.1.106:44200/core/connect/token";
//		public const string BotaNaRodaEndpoint = "http://192.168.1.106:44200/api";

//		public const string IdSvrAuthorizeEndpoint = "http://lucas-pc:42000/core/connect/authorize";
//		public const string IdSvrTokenEndpoint = "http://lucas-pc:42000/core/connect/token";
//		public const string BotaNaRodaEndpoint = "http://lucas-pc:42000/api";
    }
}