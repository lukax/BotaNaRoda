using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer3.Core;
using Newtonsoft.Json.Linq;

namespace BotaNaRoda.WebApi.Identity
{
    public class FacebookUtil
    {
        public const string XmlSchemaString = "http://www.w3.org/2001/XMLSchema#string";
        public const string Issuer = "Facebook";

        /// <summary>
        /// Get claims for name, email and picture
        /// </summary>
        /// <param name="facebookAccessToken">access token with public_profile and email scope</param>
        /// <returns>claims</returns>
        public static async Task<List<Claim>> GetClaimsAsync(string facebookAccessToken)
        {
            string userInformationEndpoint = "https://graph.facebook.com/me?fields=name,email,picture&access_token=" + Uri.EscapeDataString(facebookAccessToken);

            HttpResponseMessage graphResponse = await new HttpClient().GetAsync(userInformationEndpoint);
            graphResponse.EnsureSuccessStatusCode();
            var text = await graphResponse.Content.ReadAsStringAsync();
            JObject user = JObject.Parse(text);

            var claims = new List<Claim>();
            foreach (var param in user)
            {
                if (param.Key == "id")
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, param.Value.ToString(), XmlSchemaString, Issuer));
                if (param.Key == "name")
                    claims.Add(new Claim(Constants.ClaimTypes.Name, param.Value.ToString(), XmlSchemaString, Issuer));
                if (param.Key == "username")
                    claims.Add(new Claim(Constants.ClaimTypes.PreferredUserName, param.Value.ToString(), XmlSchemaString, Issuer));
                if (param.Key == "email")
                    claims.Add(new Claim(Constants.ClaimTypes.Email, param.Value.ToString(), XmlSchemaString, Issuer));

                var claimType = $"urn:facebook:{param.Key}";
                string claimValue = param.Value.ToString();
                claims.Add(new Claim(claimType, claimValue, XmlSchemaString, Issuer));
            }
            //Parse facebook picture object into our custom avatar url claim
            claims.Add(new Claim(Constants.ClaimTypes.Picture, (string)user["picture"]["data"]["url"]));

            return claims;
        }
    }
}
