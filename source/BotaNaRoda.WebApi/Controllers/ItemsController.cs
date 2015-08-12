using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ItemsController : Controller
    {
        private readonly ItemsContext _itemsContext;

        public ItemsController(ItemsContext itemsContext)
        {
            _itemsContext = itemsContext;
        }

        // GET: api/items
        [HttpGet]
        public async Task<List<Item>> Get()
        {
            return await _itemsContext.Items.Find(new BsonDocument()).ToListAsync();
        }

        // GET api/items/5
        [HttpGet("{id}")]
        public async Task<Item> Get(string id)
        {
            return await _itemsContext.Items.Find(x => x.Id == id).FirstAsync();
        }

        // POST api/items
        [HttpPost]
        public async void Post([FromBody]PostItemBindingModel model)
        {
            await _itemsContext.Items.InsertOneAsync(new Item(model));
        }

        // DELETE api/items/5
        [HttpDelete("{id}")]
        public async void Delete(string id)
        {
            await _itemsContext.Items.DeleteOneAsync(x => x.Id == id);
        }
    }
}
