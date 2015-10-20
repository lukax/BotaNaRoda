using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Validation;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;

namespace BotaNaRoda.WebApi.Identity
{
    public class FacebookCustomGrantValidator : ICustomGrantValidator
    {
        private readonly ItemsContext _itemsContext;

        public FacebookCustomGrantValidator(ItemsContext itemsContext)
        {
            _itemsContext = itemsContext;
        }

        async Task<CustomGrantValidationResult> ICustomGrantValidator.ValidateAsync(ValidatedTokenRequest request)
        {
            var fbAccessToken = request.Raw.Get("access_token");
            if (string.IsNullOrWhiteSpace(fbAccessToken))
            {
                return new CustomGrantValidationResult("Missing parameters.");
            }

            ExternalIdentity externalIdentity;
            try
            {
                var fbClaims = await FacebookUtil.GetClaimsAsync(fbAccessToken);
                externalIdentity = ExternalIdentity.FromClaims(fbClaims);
            }
            catch
            {
                return new CustomGrantValidationResult("Invalid access token");
            }

            var user = await _itemsContext.Users.Find(x => x.Provider == externalIdentity.Provider && x.ProviderId == externalIdentity.ProviderId).FirstOrDefaultAsync();
            if (user == null)
            {
                // new user, so add them here
                var nameClaim = externalIdentity.Claims.First(x => x.Type == Constants.ClaimTypes.Name);
                var emailClaim = externalIdentity.Claims.First(x => x.Type.Contains(Constants.ClaimTypes.Email));
                var pictureClaim = externalIdentity.Claims.First(x => x.Type == Constants.ClaimTypes.Picture);

                user = new User
                {
                    Provider = externalIdentity.Provider,
                    ProviderId = externalIdentity.ProviderId,
                    Username = emailClaim.Value,
                    Email = emailClaim.Value,
                    Name = nameClaim.Value,
                    Avatar = pictureClaim.Value
                };
                await _itemsContext.Users.InsertOneAsync(user);
            }

            var result = new CustomGrantValidationResult(user.Id, "facebookCustomGrant", user.GetClaims(), user.Provider);
            return result;
        }

        public string GrantType => "facebook";
    }
}
