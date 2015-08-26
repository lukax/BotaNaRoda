using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer3.Core.Models;

namespace BotaNaRoda.WebApi.Identity
{
    public class Scopes
    {
        public const string ApiScope = "https://api.botanaroda.com.br";

        public static IEnumerable<Scope> Get()
        {
            return new[]
            {
                StandardScopes.OpenId,
                StandardScopes.ProfileAlwaysInclude,
                StandardScopes.EmailAlwaysInclude,

                ////////////////////////
                // resource scopes
                ////////////////////////

                new Scope
                {
                    Name = ApiScope,
                    DisplayName = "Bota na Roda API",
                    Type = ScopeType.Resource,
                }
            };
        }
    }
}
