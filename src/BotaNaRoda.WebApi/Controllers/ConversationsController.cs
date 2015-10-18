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
            var currentUserId = User.GetSubjectId();

            var conversations = await _itemsContext.Conversations
                .Find(x => x.ToUserId == currentUserId || x.FromUserId == currentUserId).ToListAsync();

            var itemIds = conversations.Select(x => x.ItemId);

            HashSet<string> userIds = new HashSet<string>();
            foreach (var id in conversations.Select(x => x.ToUserId))
            {
                userIds.Add(id);
            }
            foreach (var id in conversations.Select(x => x.FromUserId))
            {
                userIds.Add(id);
            }

            var items = await _itemsContext.Items
                .Find(Builders<Item>.Filter.Where(x => itemIds.Contains(x.Id))).ToListAsync();
            var users = await _itemsContext.Users
                .Find(Builders<User>.Filter.Where(x => userIds.Contains(x.Id))).ToListAsync();

            return conversations
                .Select(x =>
            {
                var item = items.First(i => i.Id == conversations.First(c => c.Id == x.Id).ItemId);

                var receivingEndUser = users.First(usr => usr.Id == x.GetReceivingEndUserId(currentUserId));

                return new ConversationListViewModel
                {
                    Id = x.Id,
                    ItemName = item.Name,
                    ItemThumbImage = item.ThumbImage.Url,
                    LastUpdated = item.LastUpdated(),
                    ToUserName = receivingEndUser.Name
                };
            });
        }

        // GET api/conversations/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var conversation = await _itemsContext.Conversations.Find(x => x.Id == id).FirstOrDefaultAsync();

            var currentUserId = User.GetSubjectId();
            if (conversation == null || !(conversation.ToUserId == currentUserId || conversation.FromUserId == currentUserId))
            {
                return HttpNotFound();
            }

            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == conversation.GetReceivingEndUserId(currentUserId)).FirstAsync();

            var item = await _itemsContext.Items.Find(x => x.Id == conversation.Id).FirstAsync();

            return new ObjectResult(new ConversationDetailViewModel
            {
                Id = conversation.Id,
                ToUserName = receivingEndUser.Name,
                ToUserAvatar = receivingEndUser.Avatar,
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
