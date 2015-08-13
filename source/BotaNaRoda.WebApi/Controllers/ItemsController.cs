using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Domain;
using BotaNaRoda.WebApi.Models;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Thinktecture.IdentityServer.Core.Extensions;

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
        public async Task<IActionResult> Get(string id)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == id).FirstAsync();
            if (item == null)
            {
                return HttpNotFound();
            }

            var user = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();
            return new ObjectResult(new ListItemViewModel(item, new UserViewModel(user)));
        }

        // POST api/items
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Post([FromBody] PostItemBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var item = new Item(model, User.GetSubjectId());
            await _itemsContext.Items.InsertOneAsync(item);
            return Created(Request.Path + item.Id, null);
        }

        // DELETE api/items/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            var userId = User.GetSubjectId();
            var result = await _itemsContext.Items.DeleteOneAsync(x => x.Id == id && x.UserId == userId);
            //TODO acknowledge result
            //if (result.DeletedCount > 0)
            //{
                return new HttpStatusCodeResult(204); // No content
            //}
            //return HttpNotFound();
        }
    }
}
