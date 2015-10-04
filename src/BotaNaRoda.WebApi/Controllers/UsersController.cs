using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using IdentityServer3.Core.Extensions;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ItemsContext _itemsContext;
        private readonly ILogger<UsersController> _logger;

        public UsersController(ItemsContext itemsContext, ILogger<UsersController> logger)
        {
            _itemsContext = itemsContext;
            _logger = logger;
        }

        // GET api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            if (id == "me")
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return HttpUnauthorized();
                }
                id = User.GetSubjectId();
            }

            var user = await _itemsContext.Users.Find(x => x.Id == id).FirstAsync();
            if (user == null)
            {
                _logger.LogError("Could not find user by id: " + id);
                return HttpNotFound();
            }

            return new ObjectResult(new UserViewModel(user));
        }

        // POST api/users
        //[HttpPost]
        //public async Task<IActionResult> Register([FromBody] RegisterUserBindingModel model)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        _logger.LogError("Tried to register user with invalid model. " + ModelState.ToJson());
        //        return HttpBadRequest(ModelState);
        //    }

        //    var user = new User(model);
        //    user.PasswordHash = new PasswordHasher<User>().HashPassword(user, model.Password);

        //    await _itemsContext.Users.InsertOneAsync(user);

        //    return Created(Request.Path + "/" + user.Id, null);
        //}
    }
}
