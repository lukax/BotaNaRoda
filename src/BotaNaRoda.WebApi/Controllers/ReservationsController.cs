using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using BotaNaRoda.WebApi.Util;
using IdentityServer3.Core.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class ReservationsController : Controller
    {
        private readonly ILogger<ReservationsController> _logger;
        private readonly ItemsContext _itemsContext;
        private readonly NotificationService _notificationService;

        public ReservationsController(ILogger<ReservationsController> logger, 
            ItemsContext itemsContext, NotificationService notificationService)
        {
            _logger = logger;
            _itemsContext = itemsContext;
            _notificationService = notificationService;
        }

        [HttpPost("{itemId}/Promise/{userId}")]
        public async Task<IActionResult> PromiseItem(string itemId, string userId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound("Unable to find item with id: " + itemId);
            }
            if (item.Status == ItemStatus.Unavailable)
            {
                return HttpBadRequest("Item is not available");
            }
            if (item.UserId != User.GetSubjectId())
            {
                return HttpBadRequest("You're not the owner");
            }

            //Update item status
            item.UpdatedAt = DateProvider.Get;
            item.Status = ItemStatus.Pending;
            item.ReservedBy = userId;

            //TODO use bulk write
            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .Set(x => x.Status, item.Status)
                    .Set(x => x.ReservedBy, item.ReservedBy));

            var itemConversation =
                await _itemsContext.Conversations.Find(
                    x => x.ItemId == item.Id && x.FromUserId == User.GetSubjectId() && x.ToUserId == userId).FirstOrDefaultAsync();

            if (itemConversation == null)
            {
                //Create conversation
                itemConversation = new Conversation
                {
                    ItemId = item.Id,
                    FromUserId = User.GetSubjectId(),
                    ToUserId = userId,
                };

                await _itemsContext.Conversations.InsertOneAsync(itemConversation);
                _notificationService.OnItemPromise(item);
            }

            return new JsonResult(itemConversation.Id);
        }

        [HttpDelete("{itemId}/Promise/{userId}")]
        public async Task<IActionResult> PromiseDeny(string itemId, string userId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound("Unable to find item with id: " + itemId);
            }
            if (item.UserId != User.GetSubjectId())
            {
                return HttpBadRequest("You're not the owner");
            }

            item.UpdatedAt = DateProvider.Get;
            item.Status = ItemStatus.Available;
            item.ReservedBy = null;

            await _itemsContext.Conversations.DeleteOneAsync(x => x.ItemId == item.Id);

            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .Set(x => x.ReservedBy, item.ReservedBy));

            _notificationService.OnItemPromiseDenied(item);

            return new HttpOkResult();
        }

        [HttpPost("{itemId}/Subscribe")]
        public async Task<IActionResult> Subscribe(string itemId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound("Unable to find item with id: " + itemId);
            }

            if (item.Status == ItemStatus.Unavailable)
            {
                return HttpBadRequest("Item is not available");
            }
            if (item.UserId == User.GetSubjectId())
            {
                return HttpBadRequest("You cannot subscribe to this item");
            }
            if (item.Subscribers != null && item.Subscribers.Contains(User.GetSubjectId()))
            {
                return HttpBadRequest("You are already subscribed");
            }

            item.UpdatedAt = DateProvider.Get;

            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .AddToSet(x => x.Subscribers, User.GetSubjectId()));

            _notificationService.OnItemSubscribe(item, User.GetSubjectId());

            return new HttpOkResult();
        }

        [HttpDelete("{itemId}/Unsubscribe")]
        public async Task<IActionResult> Unubscribe(string itemId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound("Unable to find item with id: " + itemId);
            }

            if (item.Status == ItemStatus.Unavailable)
            {
                return HttpBadRequest("Item is not available");
            }
            if (item.UserId == User.GetSubjectId())
            {
                return HttpBadRequest("You are already subscribed to your own item");
            }

            item.UpdatedAt = DateTime.UtcNow;
            item.Subscribers.Remove(User.GetSubjectId());

            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .Set(x => x.Subscribers, item.Subscribers));

            _notificationService.OnItemUnsubscribe(item, User.GetSubjectId());

            return new HttpOkResult();
        }

        [HttpPost("{itemId}/Receive")]
        public async Task<IActionResult> Receive(string itemId, [FromBody] UserReviewBindingModel reviewBindingModel)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                _logger.LogError("Unable to find item with id: " + itemId);
                return HttpNotFound();
            }

            if (reviewBindingModel == null 
                || reviewBindingModel.Score < 0 
                || reviewBindingModel.Score > 5)
            {
                return HttpBadRequest("Invalid review");
            }

            if (item.Status == ItemStatus.Pending && item.ReservedBy == User.GetSubjectId())
            {
                _notificationService.OnItemReceived(item);

                //Update item status
                item.UpdatedAt = DateProvider.Get;
                item.Status = ItemStatus.Unavailable;
                item.Subscribers = new List<string>();

                await _itemsContext.Items.UpdateOneAsync(
                    Builders<Item>.Filter.Eq(x => x.Id, itemId),
                    Builders<Item>.Update
                        .Set(x => x.UpdatedAt, item.UpdatedAt)
                        .Set(x => x.Status, item.Status)
                        .Set(x => x.Subscribers, item.Subscribers));

                //Add review
                var review = new UserReview
                {
                    FromUserId = User.GetSubjectId(),
                    Message = reviewBindingModel.Message,
                    Score = reviewBindingModel.Score
                };
                await _itemsContext.Users.UpdateOneAsync(
                    Builders<User>.Filter.Eq(x => x.Id, item.UserId),
                    Builders<User>.Update
                        .AddToSet(x => x.Reviews, review));

                
                return new HttpOkResult();
            }

            return HttpBadRequest("Cannot receive item");
        }
    }
}
