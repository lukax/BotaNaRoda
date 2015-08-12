using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Framework.OptionsModel;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Data
{
    public class ItemsContext
    {
        private IMongoDatabase _db;

        public ItemsContext(IOptions<AppSettings> appSettings)
        {
            var client = new MongoClient(appSettings.Options.BotaNaRodaConnectionString);
            _db = client.GetDatabase(appSettings.Options.BotaNaRodaDatabaseName);
        }
    }
}
