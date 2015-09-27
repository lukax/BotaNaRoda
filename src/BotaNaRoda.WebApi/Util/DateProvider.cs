using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;

namespace BotaNaRoda.WebApi.Util
{
    public static class DateProvider
    {
        public static DateTime Get => DateTime.UtcNow;

        public static DateTime LastUpdated(this IUpdatable updatable)
        {
            return updatable.UpdatedAt == default(DateTime) ? updatable.CreatedAt : updatable.UpdatedAt;
        }
    }
}
