
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
                conversation.HubInfo.FromUserIsConnected = true;
                viewModel.ToUser = new UserViewModel(toUsr);
            }
            else
            {
                conversation.HubInfo.ToUserConnectionId = Context.ConnectionId;
                conversation.HubInfo.ToUserIsConnected = true;
                viewModel.ToUser = new UserViewModel(fromUsr);
            }

            await _context.Conversations.UpdateOneAsync(
                Builders<Conversation>.Filter.Eq(x => x.Id, conversation.Id),
                Builders<Conversation>.Update.Set(x => x.HubInfo, conversation.HubInfo));

            Clients.CallerState.conversationId = conversationId;

            return viewModel;
        }

        public async Task SendMessage(SendConversationMessageBindingModel sendMessageModel)
        {
            var currentUserId = Context.User.GetSubjectId();

            if (string.IsNullOrEmpty(sendMessageModel.Message))
            {
                throw new HubException("Invalid message");
            }

            var conversation = await _context.Conversations
                .Find(x => x.Id == sendMessageModel.ConversationId && (x.FromUserId == currentUserId || x.ToUserId == currentUserId))
                .FirstOrDefaultAsync();
            if (conversation != null)
            {
                //add msg to conversation
                await _context.Conversations.UpdateOneAsync(
                    Builders<Conversation>.Filter.Eq(x => x.Id, sendMessageModel.ConversationId),
                    Builders<Conversation>.Update.AddToSet(x => x.Messages, new ConversationChatMessage
                    {
                        Message = sendMessageModel.Message,
                        SentBy = currentUserId
                    }));

                Clients.Clients(new [] { conversation.HubInfo.ToUserConnectionId, conversation.HubInfo.FromUserConnectionId})
                    .OnMessageReceived(new ConversationChatMessageViewModel
                    {
                        Message = sendMessageModel.Message,
                        SentAt = DateProvider.Get,
                        SentBy = currentUserId
                    });


                _notificationService.OnConversationMessageSent(conversation, currentUserId);
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            var currentUserId = Context.User.GetSubjectId();
            string conversationId = Clients.CallerState.conversationId;
            if (conversationId != null)
            {
                var conversation = await _context.Conversations.Find(x => x.Id == conversationId).FirstOrDefaultAsync();

                if (conversation.FromUserId == currentUserId)
                {
                    await _context.Conversations
                        .UpdateOneAsync(x => x.Id == conversationId, Builders<Conversation>.Update.Set(x => x.HubInfo.FromUserIsConnected, false));
                }
                else
                {
                    await _context.Conversations
                        .UpdateOneAsync(x => x.Id == conversationId, Builders<Conversation>.Update.Set(x => x.HubInfo.ToUserIsConnected, false));
                }
            }
        }

    }
}