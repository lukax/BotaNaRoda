using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;
using Microsoft.Framework.OptionsModel;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Data
{
    public class ItemsContext
    {
        private readonly IMongoDatabase _db;

        public ItemsContext(IOptions<AppSettings> appSettings)
        {
            var client = new MongoClient(appSettings.Options.BotaNaRodaConnectionString);
            _db = client.GetDatabase(appSettings.Options.BotaNaRodaDatabaseName);
        }

        public IMongoCollection<Item> Items => _db.GetCollection<Item>("items");

        public IMongoCollection<User> Users => _db.GetCollection<User>("users");
    }
}
