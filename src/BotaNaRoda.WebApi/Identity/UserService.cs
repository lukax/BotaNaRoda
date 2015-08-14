using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Domain;
using Microsoft.AspNet.Identity;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Extensions;
using Thinktecture.IdentityServer.Core.Models;
using Thinktecture.IdentityServer.Core.Services;

namespace BotaNaRoda.WebApi.Identity
{
    public class UserService : IUserService
    {
        public static List<User> Users = new List<User>()
        {
            new User{
                Id = "123",
                Username = "alice",
                PasswordHash = "AQAAAAEAACcQAAAAEJDhtBV5eN45xYhcm+hnnvr6u1uhqLrsDFZcMGnCSx3YDpRhyOhzRpDTCgeC1odrxg==",
            },
            new User{
                Id = "890",
                Username = "bob",
                PasswordHash = "AQAAAAEAACcQAAAAEL+DbblrZq5vYstzBdDtGz8uAkT7ZQT2pMNseXh2AL19gxqxaFREx6pcIGsyc0vi7w==",
            },
        };

        public Task<AuthenticateResult> AuthenticateExternalAsync(ExternalIdentity externalUser, SignInMessage message)
        {
            return Task.FromResult<AuthenticateResult>(null);
        }

        public Task<AuthenticateResult> AuthenticateLocalAsync(string username, string password, SignInMessage message)
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();
            var user = Users.SingleOrDefault(x => x.Username == username 
                && hasher.VerifyHashedPassword(x, x.PasswordHash, password) != PasswordVerificationResult.Failed);
            if (user == null)
            {
                return Task.FromResult<AuthenticateResult>(null);
            }

            return Task.FromResult(new AuthenticateResult(user.Id, user.Username));
        }

        public Task<IEnumerable<Claim>> GetProfileDataAsync(ClaimsPrincipal subject, IEnumerable<string> requestedClaimTypes = null)
        {
            // issue the claims for the user
            var user = Users.SingleOrDefault(x => x.Id == subject.GetSubjectId());
            if (user == null)
            {
                return Task.FromResult<IEnumerable<Claim>>(null);
            }

            //TODO setup claims
            return Task.FromResult(new List<Claim>
            {
                new Claim(Constants.ClaimTypes.PreferredUserName, user.Username),
            }.AsEnumerable());
        }

        public Task<bool> IsActiveAsync(ClaimsPrincipal subject)
        {
            var user = Users.SingleOrDefault(x => x.Id == subject.GetSubjectId());
            return Task.FromResult(user != null);
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
