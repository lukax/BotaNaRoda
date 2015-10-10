using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace BotaNaRoda.WebApi.Identity
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                //TODO Verify origin
                new Client
                {
                    ClientName = "Bota na Roda",
                    ClientId = "android.botanaroda.com.br",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("150D23DAE26C4629BC58A7104B1AECFB".Sha256()) //Dummy secret as requirement of the hybrid flow
                    },
                    Flow = Flows.AuthorizationCode, //Allows the retrieval of both: code, id_token and token
                    ClientUri = "https://botanaroda.com.br",
                    RequireConsent = false,
                    AllowAccessToAllScopes = true, //<------------TODO review this
                    RedirectUris = new List<string>
                    {
                        "https://botanaroda.azurewebsites.net/core",
                        "http://192.168.1.106:44200/core",
                    },
                    IdentityTokenLifetime = 360 /* 5Min */,
                    AccessTokenLifetime = 36000 * 10 /* 1Hour */,
                    RefreshTokenUsage = TokenUsage.ReUse,
                    SlidingRefreshTokenLifetime = 1296000 * 2 /* 30Days */
                },
#if DEBUG
                new Client
                {
                    ClientName = "Hybrid Client Demo",
                    Enabled = true,

                    ClientId = "hybridclient",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    Flow = Flows.AuthorizationCode,

                    ClientUri = "http://www.thinktecture.com",
                    RequireConsent = true,
                    AllowRememberConsent = true,
                    AllowAccessToAllScopes = true,
                    RedirectUris = new List<string>
                    {
                        // "simple JS client"
                        "http://localhost:37045/index.html",
                        "https://localhost:44331/Home/Callback",

                        // OAuthJS client
                        "http://localhost:23453/callback.html",
                        "http://localhost:23453/frame.html",
                        "http://localhost:23453/modal.html",

                        "https://localhost:44333/core",
                        "http://localhost:42000/core",
                        "http://192.168.1.106:42001/core",
                        "http://botanarodaapi.azurewebsites.net/core",

                        // WPF client
                        "oob://localhost/wpfclient",

                        // WinRT client
                        "ms-app://s-1-15-2-1677770454-1667073387-2045065244-1646983296-4049597744-3433330513-3528227871/",

                        // JavaScript client
                        "http://localhost:21575/index.html",
                        "http://localhost:21575/silent_renew.html",

                        // MVC form post sample
                        "http://localhost:11716/account/signInCallback",

                        // OWIN middleware client
                        "http://localhost:2671/",
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:23453/index.html",
                        "http://localhost:21575/index.html"
                    },

                    AllowedCorsOrigins = new List<string>{
                        "http://localhost:21575"
                    },

                    IdentityTokenLifetime = 360,
                    AccessTokenLifetime = 3600,
                },

                new Client
                {
                    ClientName = "Implicit Client Demo",
                    Enabled = true,

                    ClientId = "implicitclient",
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("secret".Sha256())
                    },

                    Flow = Flows.Implicit,

                    ClientUri = "http://www.thinktecture.com",
                    RequireConsent = true,
                    AllowRememberConsent = true,
                    AllowAccessToAllScopes = true,
                    RedirectUris = new List<string>
                    {
                        // "simple JS client"
                        "http://localhost:37045/index.html",
                        "https://localhost:44331/Home/Callback",

                        // OAuthJS client
                        "http://localhost:23453/callback.html",
                        "http://localhost:23453/frame.html",
                        "http://localhost:23453/modal.html",

                        "https://localhost:44333/core",
                        "http://localhost:42000/core",
                        "http://192.168.1.106:42001/core",
                        "http://botanarodaapi.azurewebsites.net/core",

                        // WPF client
                        "oob://localhost/wpfclient",

                        // WinRT client
                        "ms-app://s-1-15-2-1677770454-1667073387-2045065244-1646983296-4049597744-3433330513-3528227871/",

                        // JavaScript client
                        "http://localhost:21575/index.html",
                        "http://localhost:21575/silent_renew.html",

                        // MVC form post sample
                        "http://localhost:11716/account/signInCallback",

                        // OWIN middleware client
                        "http://localhost:2671/",
                    },

                    PostLogoutRedirectUris = new List<string>
                    {
                        "http://localhost:23453/index.html",
                        "http://localhost:21575/index.html"
                    },

                    AllowedCorsOrigins = new List<string>{
                        "http://localhost:21575"
                    },

                    IdentityTokenLifetime = 360,
                    AccessTokenLifetime = 3600,
                },
#endif
            };
        }
    }
}
