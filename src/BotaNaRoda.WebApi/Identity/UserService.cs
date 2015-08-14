using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Domain;
using Microsoft.AspNet.Identity;
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

        public Task<AuthenticateResult> AuthenticateExternalAsync(ExternalIdentity externalUser, SignInMessage message)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public async Task<AuthenticateResult> AuthenticateLocalAsync(string username, string password, SignInMessage message)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            User user = await _itemsContext.Users.Find(x => x.Username == username &&
                                          (hasher.VerifyHashedPassword(x, x.PasswordHash, password) !=
                                           PasswordVerificationResult.Failed)).FirstOrDefaultAsync();

            if (user == null)
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
