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
        private readonly ItemsContext _itemsContext;
        private readonly ILogger<ReservationsController> _logger;

        public ReservationsController(ItemsContext itemsContext, ILogger<ReservationsController> logger)
        {
            _itemsContext = itemsContext;
            _logger = logger;
        }

        [HttpPost("{itemId}")]
        public async Task<IActionResult> ReserveItem(string itemId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound("Unable to find item with id: " + itemId);
            }

            if (item.Status != ItemStatus.Available)
            {
                return HttpBadRequest("Item is not available");
            }
            if (item.UserId == User.GetSubjectId())
            {
                return HttpBadRequest("You are not eligible to reserve the item");
            }

            //Update item status
            item.UpdatedAt = DateProvider.Get;
            item.Status = ItemStatus.Pending;
            item.ReservedBy = User.GetSubjectId();

            //TODO use bulk write
            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .Set(x => x.Status, item.Status)
                    .Set(x => x.ReservedBy, item.ReservedBy));

            //Create conversation
            var itemConversation = new Conversation
            {
                ItemId = item.Id,
                FromUserId = User.GetSubjectId(),
                ToUserId = item.UserId,
            };

            await _itemsContext.Conversations.InsertOneAsync(itemConversation);

            return new JsonResult(itemConversation.Id);
        }

        [HttpDelete("{itemId}")]
        public async Task<IActionResult> CancelReservation(string itemId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == itemId).FirstOrDefaultAsync();
            if (item == null)
            {
                return HttpNotFound("Unable to find item with id: " + itemId);
            }

            if (item.ReservedBy != User.GetSubjectId())
            {
                return HttpBadRequest("Item was not reserved");
            }

            item.UpdatedAt = DateProvider.Get;
            item.Status = ItemStatus.Available;
            item.ReservedBy = null;

            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .Set(x => x.ReservedBy, item.ReservedBy));

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
                return HttpBadRequest("You are already subscribed to your own item");
            }

            item.UpdatedAt = DateTime.UtcNow;
            var currentUserId = User.GetSubjectId();

            var result = await _itemsContext.Items.UpdateOneAsync(
                Builders<Item>.Filter.Eq(x => x.Id, itemId),
                Builders<Item>.Update
                    .Set(x => x.UpdatedAt, item.UpdatedAt)
                    .AddToSet(x => x.Subscribers, currentUserId));

            return new HttpOkResult();
        }

        [HttpPost("{itemId}/Received")]
        public async Task<IActionResult> Received(string itemId, [FromBody] UserReviewBindingModel reviewBindingModel)
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
                //Update item status
                item.UpdatedAt = DateTime.UtcNow;
                item.Status = ItemStatus.Unavailable;

                await _itemsContext.Items.UpdateOneAsync(
                    Builders<Item>.Filter.Eq(x => x.Id, itemId),
                    Builders<Item>.Update
                        .Set(x => x.UpdatedAt, item.UpdatedAt)
                        .Set(x => x.Status, item.Status));

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
