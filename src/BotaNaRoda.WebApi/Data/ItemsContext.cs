using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;
using Microsoft.Framework.Configuration;
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

            CreateIndexes();
        }

        public IMongoCollection<Item> Items => _db.GetCollection<Item>("items");

        public IMongoCollection<User> Users => _db.GetCollection<User>("users");

        public IMongoCollection<Conversation> Conversations => _db.GetCollection<Conversation>("conversations");

        private async void CreateIndexes()
        {
            await Items.Indexes.CreateOneAsync(Builders<Item>.IndexKeys.Geo2DSphere(x => x.Loc));
            await Items.Indexes.CreateOneAsync(Builders<Item>.IndexKeys.Ascending(x => x.Status));
            await Users.Indexes.CreateOneAsync(Builders<User>.IndexKeys.Geo2DSphere(x => x.Loc));
        }
    }
}
