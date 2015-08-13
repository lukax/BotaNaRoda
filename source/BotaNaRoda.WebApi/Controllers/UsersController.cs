using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Domain;
using BotaNaRoda.WebApi.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : Controller
    {
        private readonly ItemsContext _itemsContext;

        public UsersController(ItemsContext itemsContext)
        {
            _itemsContext = itemsContext;
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var user = await _itemsContext.Users.Find(x => x.Id == id).FirstAsync();
            if (user == null)
            {
                return HttpNotFound();
            }

            return new ObjectResult(new UserViewModel(user));
        }

        // POST api/users
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegisterUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var user = new User(model);
            user.PasswordHash = new PasswordHasher<User>().HashPassword(user, model.Password);

            await _itemsContext.Users.InsertOneAsync(user);

            return Created(Request.Path + user.Id, null);
        }
    }
}
