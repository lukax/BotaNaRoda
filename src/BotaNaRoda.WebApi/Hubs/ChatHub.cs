
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using BotaNaRoda.WebApi.Util;
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
            var currentUserId = Context.User.GetSubjectId();

            var conversation = await _context.Conversations
                .Find(x => x.Id == conversationId && (x.FromUserId == currentUserId || x.ToUserId == currentUserId))
                .FirstOrDefaultAsync();
            if (conversation == null)
            {
                throw new HubException("Conversation not found");
            }

            var item = await _context.Items.Find(x => x.Id == conversation.ItemId).FirstOrDefaultAsync();
            var fromUsr = await _context.Users.Find(x => x.Id == conversation.FromUserId).FirstOrDefaultAsync();
            var toUsr = await _context.Users.Find(x => x.Id == conversation.ToUserId).FirstOrDefaultAsync();

            //update conversation hub info
            conversation.HubInfo = conversation.HubInfo ?? new ConversationHubInfo();

            var viewModel = new ConversationChatConnectionViewModel
            {
                UpdatedAt = conversation.UpdatedAt,
                Messages = conversation.Messages.Select(x => new ConversationChatMessageViewModel
                {
                    Message = x.Message,
                    SentAt = x.SentAt,
                    SentBy = x.SentBy
                }).ToList(),
                Item = new ItemListViewModel(item)
            };

            if (conversation.FromUserId == currentUserId)
            {
                conversation.HubInfo.FromUserConnectionId = Context.ConnectionId;
                viewModel.ToUser = new UserViewModel(toUsr);
            }
            else
            {
                conversation.HubInfo.ToUserConnectionId = Context.ConnectionId;
                viewModel.ToUser = new UserViewModel(fromUsr);
            }

            await _context.Conversations.UpdateOneAsync(
                Builders<Conversation>.Filter.Eq(x => x.Id, conversation.Id),
                Builders<Conversation>.Update.Set(x => x.HubInfo, conversation.HubInfo));

            return viewModel;
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
                    Builders<Conversation>.Update.AddToSet(x => x.Messages, new ConversationChatMessage
                    {
                        Message = sendMessageModel.Message,
                        SentBy = userId
                    }));

                Clients.Clients(new [] { conversation.HubInfo.ToUserConnectionId, conversation.HubInfo.FromUserConnectionId})
                    .OnMessageReceived(new ConversationChatMessageViewModel
                    {
                        Message = sendMessageModel.Message,
                        SentAt = DateProvider.Get,
                        SentBy = userId
                    });

                _notificationService.OnConversationMessageSent(conversation, userId);
            }
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }

    }
}