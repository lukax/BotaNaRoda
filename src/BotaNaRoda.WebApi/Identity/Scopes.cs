using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace BotaNaRoda.WebApi.Identity
{
    public class Scopes
    {
        public const string BotaNaRodaApiScope = "https://api.botanaroda.com.br";

        public static IEnumerable<Scope> Get()
        {
            return new[]
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,
                StandardScopes.Email,
                StandardScopes.Address,
                StandardScopes.OfflineAccess,

                ////////////////////////
                // resource scopes
                ////////////////////////

                new Scope
                {
                    Name = BotaNaRodaApiScope,
                    DisplayName = "Bota na Roda API",
                    Type = ScopeType.Resource,
                }
            };
        }
    }
}
