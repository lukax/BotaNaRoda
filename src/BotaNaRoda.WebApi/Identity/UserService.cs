using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Domain;
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using MongoDB.Driver;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace BotaNaRoda.WebApi.Identity
{
    public class UserService : IUserService
    {
        private readonly ItemsContext _itemsContext;

        public UserService(ItemsContext itemsContext)
        {
            _itemsContext = itemsContext;
        }

        public async Task<AuthenticateResult> AuthenticateExternalAsync(ExternalIdentity externalUser, SignInMessage message)
        {
            // look for the user in our local identity system from the external identifiers
            var user = await _itemsContext.Users.Find(x => x.Provider == externalUser.Provider && x.ProviderId == externalUser.ProviderId).FirstOrDefaultAsync();

            if (user == null)
            {
                // new user, so add them here
                var nameClaim = externalUser.Claims.First(x => x.Type == Constants.ClaimTypes.Name);
                var emailClaim = externalUser.Claims.First(x => x.Type == Constants.ClaimTypes.Email);
                var avatarClaim = externalUser.Claims.FirstOrDefault(x => x.Type == "avatar");

                user = new User
                {
                    Provider = externalUser.Provider,
                    ProviderId = externalUser.ProviderId,
                    Username = emailClaim.Value,
                    Name = nameClaim.Value,
                    Avatar = avatarClaim?.Value
                };
                await _itemsContext.Users.InsertOneAsync(user);
            }

            return await Task.FromResult(new AuthenticateResult(user.Id, user.Username, identityProvider: user.Provider));
        }

        public async Task<AuthenticateResult> AuthenticateLocalAsync(string username, string password, SignInMessage message)
        {
            var user = await _itemsContext.Users.Find(x => x.Username == username).FirstOrDefaultAsync();

            PasswordHasher<User> hasher = new PasswordHasher<User>();
            if (user == null || hasher.VerifyHashedPassword(user, user.PasswordHash, password) == PasswordVerificationResult.Failed)
            {
                return await Task.FromResult<AuthenticateResult>(null);
            }

            return await Task.FromResult(new AuthenticateResult(user.Id, user.Username));
        }

        public async Task<IEnumerable<Claim>> GetProfileDataAsync(ClaimsPrincipal subject, IEnumerable<string> requestedClaimTypes = null)
        {
            // issue the claims for the user
            User user = await _itemsContext.Users.Find(x => x.Id == subject.GetSubjectId()).FirstOrDefaultAsync();
            if (user == null)
            {
                return await Task.FromResult<IEnumerable<Claim>>(null);
            }

            //TODO setup claims
            return await Task.FromResult(new List<Claim>
            {
                new Claim(Constants.ClaimTypes.PreferredUserName, user.Username),
            }.AsEnumerable());
        }

        public async Task<bool> IsActiveAsync(ClaimsPrincipal subject)
        {
            User user = await _itemsContext.Users.Find(x => x.Id == subject.GetSubjectId()).FirstOrDefaultAsync();
            return await Task.FromResult(user != null);
        }

        public Task<AuthenticateResult> PreAuthenticateAsync(SignInMessage message)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public Task SignOutAsync(ClaimsPrincipal subject)
        {
            return Task.FromResult(0);
        }
    }
}
