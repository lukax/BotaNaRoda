using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.OptionsModel;
using MongoDB.Driver;

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

        // GET: api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
