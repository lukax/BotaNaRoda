using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using IdentityServer3.Core;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using IdentityServer3.Core.Services;
using IdentityServer3.Core.Services.Default;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Identity
{
    public class UserService : UserServiceBase
    {
        private readonly ItemsContext _itemsContext;

        public UserService(ItemsContext itemsContext)
        {
            _itemsContext = itemsContext;
        }

        public override async Task AuthenticateLocalAsync(LocalAuthenticationContext context)
        {
            var user = await _itemsContext.Users.Find(x => x.Username == context.UserName).FirstOrDefaultAsync();

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            if (user == null ||
                hasher.VerifyHashedPassword(user, user.PasswordHash, context.Password) ==
                PasswordVerificationResult.Failed)
            {
                context.AuthenticateResult = new AuthenticateResult("Invalid username and password");
            }
            else
            {
                context.AuthenticateResult = new AuthenticateResult(user.Id, user.Username);
            }
        }

        public override async Task AuthenticateExternalAsync(ExternalAuthenticationContext context)
        {
            // look for the user in our local identity system from the external identifiers
            var user = await _itemsContext.Users.Find(x => x.Provider == context.ExternalIdentity.Provider && x.ProviderId == context.ExternalIdentity.ProviderId).FirstOrDefaultAsync();
            if (user == null)
            {
                // new user, so add them here
                var nameClaim = context.ExternalIdentity.Claims.First(x => x.Type == Constants.ClaimTypes.Name);
                var emailClaim = context.ExternalIdentity.Claims.First(x => x.Type.Contains(Constants.ClaimTypes.Email));
                var pictureClaim = context.ExternalIdentity.Claims.First(x => x.Type == Constants.ClaimTypes.Picture);

                user = new User
                {
                    Provider = context.ExternalIdentity.Provider,
                    ProviderId = context.ExternalIdentity.ProviderId,
                    Username = emailClaim.Value,
                    Email = emailClaim.Value,
                    Name = nameClaim.Value,
                    Avatar = pictureClaim.Value
                };
                await _itemsContext.Users.InsertOneAsync(user);
            }

            // user is registered so continue
            context.AuthenticateResult = new AuthenticateResult(user.Id, user.Name, identityProvider: user.Provider);
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {            
            User user = await _itemsContext.Users.Find(x => x.Id == context.Subject.GetSubjectId()).FirstOrDefaultAsync();
            if (user != null)
            {
                context.IssuedClaims = user.GetClaims();
            }
        }

        public override async Task IsActiveAsync(IsActiveContext context)
        {
            User user = await _itemsContext.Users.Find(x => x.Id == context.Subject.GetSubjectId()).FirstOrDefaultAsync();
            context.IsActive = user != null;
        }
    }
}
