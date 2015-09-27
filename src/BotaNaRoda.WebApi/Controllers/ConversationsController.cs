using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using BotaNaRoda.WebApi.Util;
using IdentityServer3.Core.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ConversationsController : Controller
    {
        private readonly ItemsContext _itemsContext;
        private readonly ILogger<ConversationsController> _logger;

        public ConversationsController(ItemsContext itemsContext, ILogger<ConversationsController> logger)
        {
            _itemsContext = itemsContext;
            _logger = logger;
        }

        // GET: api/conversations
        [HttpGet]
        public async Task<IEnumerable<ConversationListViewModel>> GetAll()
        {
            var userId = User.GetSubjectId();
            var user = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();

            var conversations = await _itemsContext.Conversations
                .Find(x => x.ToUserId == userId || x.FromUserId == userId).ToListAsync();

            var itemIds = conversations.Select(x => x.ItemId);
            var items = await _itemsContext.Items
                .Find(Builders<Item>.Filter.Where(x => itemIds.Contains(x.Id))).ToListAsync();

            return conversations.Select(x =>
            {
                var item = items.First(i => i.Id == conversations.First(c => c.Id == x.Id).ItemId);

                return new ConversationListViewModel
                {
                    Id = x.Id,
                    ItemName = item.Name,
                    ItemThumbImage = item.ThumbImage.Url,
                    LastUpdated = item.LastUpdated(),
                    ToUserName = user.Name
                };
            });
        }

        // GET api/conversations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var conversation = await _itemsContext.Conversations.Find(x => x.Id == id).FirstOrDefaultAsync();

            var userId = User.GetSubjectId();
            if (conversation == null || !(conversation.ToUserId == userId || conversation.FromUserId == userId))
            {
                return HttpNotFound();
            }

            var user = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();
            var item = await _itemsContext.Items.Find(x => x.Id == conversation.Id).FirstAsync();

            return new ObjectResult(new ConversationDetailViewModel
            {
                Id = conversation.Id,
                ToUserName = user.Name,
                ToUserAvatar = user.Avatar,
                Latitude = item.Loc.Latitude,
                Longitude = item.Loc.Longitude,
                ItemName = item.Name,
                ItemThumbImage = item.ThumbImage.Url,
                LastUpdated = item.LastUpdated(),
                Messages = conversation.Messages,
            });
        }


    }

}
