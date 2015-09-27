
using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using IdentityServer3.Core.Extensions;
using Microsoft.AspNet.Identity;
using MongoDB.Driver;

namespace BotaNaRoda.WebApi.Hubs
{
    public interface IChatHubClient
    {
        void OnMessageReceived(object conversationMessage);
    }

    [Authorize]
    public class ChatHub : Hub<IChatHubClient>
    {
        private readonly ItemsContext _context;
        private readonly NotificationService _notificationService;

        public ChatHub(ItemsContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<dynamic> Connect(string conversationId)
        {
            var userId = Context.User.GetSubjectId();

            var conversation = await _context.Conversations
                .Find(x => x.Id == conversationId && (x.FromUserId == userId || x.ToUserId == userId))
                .FirstOrDefaultAsync();
            if (conversation == null)
            {
                throw new HubException("Conversation not found");
            }

            var fromUsr = await _context.Users.Find(x => x.Id == conversation.FromUserId).FirstOrDefaultAsync();
            var toUsr = await _context.Users.Find(x => x.Id == conversation.ToUserId).FirstOrDefaultAsync();
            var item = await _context.Items.Find(x => x.Id == conversation.ItemId).FirstOrDefaultAsync();

            //update conversation hub info
            conversation.HubInfo = conversation.HubInfo ?? new ConversationHubInfo();
            if (conversation.FromUserId == userId)
            {
                conversation.HubInfo.FromUserConnectionId = Context.ConnectionId;
            }
            else
            {
                conversation.HubInfo.ToUserConnectionId = Context.ConnectionId;
            }
            await _context.Conversations.UpdateOneAsync(
                Builders<Conversation>.Filter.Eq(x => x.Id, conversation.Id),
                Builders<Conversation>.Update.Set(x => x.HubInfo, conversation.HubInfo));

            return new
            {
                UpdatedAt = conversation.UpdatedAt,
                From = new UserViewModel(fromUsr),
                To = new UserViewModel(toUsr),
                Messages = conversation.Messages.ToList(),
                Item = new ItemListViewModel(item)
            };
        }

        public async Task SendMessage(SendConversationMessageBindingModel sendMessageModel)
        {
            var userId = Context.User.GetSubjectId();

            if (string.IsNullOrEmpty(sendMessageModel.Message))
            {
                throw new HubException("Invalid message");
            }

            var conversation = await _context.Conversations
                .Find(x => x.Id == sendMessageModel.ConversationId && (x.FromUserId == userId || x.ToUserId == userId))
                .FirstOrDefaultAsync();
            if (conversation != null)
            {
                //add msg to conversation
                await _context.Conversations.UpdateOneAsync(
                    Builders<Conversation>.Filter.Eq(x => x.Id, sendMessageModel.ConversationId),
                    Builders<Conversation>.Update.AddToSet(x => x.Messages, new ConversationChatMessage {Message = sendMessageModel.Message}));


                Clients.Clients(new [] { conversation.HubInfo.ToUserConnectionId, conversation.HubInfo.FromUserConnectionId})
                    .OnMessageReceived(new
                    {
                        Message = sendMessageModel.Message
                    });

                var receivingEndUserId = userId == conversation.FromUserId
                    ? conversation.ToUserId
                    : conversation.FromUserId;
                _notificationService.OnConversationMessageSent(conversation, receivingEndUserId);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

    }
}