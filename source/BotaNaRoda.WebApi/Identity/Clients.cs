using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Models;

namespace BotaNaRoda.WebApi.Identity
{
    public class Clients
    {
        public static List<Client> Get()
        {
            return new List<Client>
            {
                new Client
                {
                    ClientName = "Code Flow Client Demo",
                    Enabled = true,

                    ClientId = "codeclient",
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret("secret".Sha256())
                    },

                    Flow = Flows.AuthorizationCode,

                    RequireConsent = true,
                    AllowRememberConsent = true,

                    ClientUri = "http://www.thinktecture.com",
                    RedirectUris = new List<string>
                    {
                        // MVC code client manual
                        "https://localhost:44312/callback",
                    },

                    ScopeRestrictions = new List<string>
                    {
                        Constants.StandardScopes.OpenId,
                        Constants.StandardScopes.Profile,
                        Constants.StandardScopes.Email,
                        Constants.StandardScopes.OfflineAccess,
                        "read",
                        "write"
                    },

                    AccessTokenType = AccessTokenType.Reference,
                },

                new Client
                {
                    ClientName = "Implicit Client Demo",
                    Enabled = true,

                    ClientId = "implicitclient",
                    ClientSecrets = new List<ClientSecret>
                    {
                        new ClientSecret("secret".Sha256())
                    },

                    Flow = Flows.Implicit,

                    ClientUri = "http://www.thinktecture.com",
                    RequireConsent = true,
                    AllowRememberConsent = true,

                    RedirectUris = new List<string>
                    {
                        // "simple JS client"
                        "http://localhost:37045/index.html",
                        "https://localhost:44331/Home/Callback",

                        // OAuthJS client
                        "http://localhost:23453/callback.html",
                        "http://localhost:23453/frame.html",
                        "http://localhost:23453/modal.html",

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
                    AccessTokenLifetime = 3600
                },
            };
        }
    }
}
