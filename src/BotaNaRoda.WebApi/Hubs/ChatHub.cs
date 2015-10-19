
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
            Clients.CallerState.conversationId = conversationId;

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
                conversation.FromUserHubInfo.ConnectionId = Context.ConnectionId;
                conversation.FromUserHubInfo.IsConnected = true;
                viewModel.ToUser = new UserViewModel(toUsr);
            }
            else
            {
                conversation.ToUserHubInfo.ConnectionId = Context.ConnectionId;
                conversation.ToUserHubInfo.IsConnected = true;
                viewModel.ToUser = new UserViewModel(fromUsr);
            }

            await _context.Conversations.UpdateOneAsync(
                Builders<Conversation>.Filter.Eq(x => x.Id, conversation.Id),
                Builders<Conversation>.Update
                    .Set(x => x.FromUserHubInfo, conversation.FromUserHubInfo)
                    .Set(x => x.ToUserHubInfo, conversation.ToUserHubInfo));

            return viewModel;
        }

        public async Task SendMessage(SendConversationMessageBindingModel sendMessageModel)
        {
            if (string.IsNullOrEmpty(sendMessageModel.Message))
            {
                throw new HubException("Invalid message");
            }

            var currentUserId = Context.User.GetSubjectId();

            var conversation = await _context.Conversations
                .Find(x => x.Id == sendMessageModel.ConversationId && (x.FromUserId == currentUserId || x.ToUserId == currentUserId))
                .FirstOrDefaultAsync();
            if (conversation == null)
            {
                throw new HubException("Conversation not found");
            }

            //add msg to conversation
            await _context.Conversations.UpdateOneAsync(
                Builders<Conversation>.Filter.Eq(x => x.Id, sendMessageModel.ConversationId),
                Builders<Conversation>.Update.AddToSet(x => x.Messages, new ConversationChatMessage
                {
                    Message = sendMessageModel.Message,
                    SentBy = currentUserId
                }));

            Clients.Clients(new[]
            {conversation.FromUserHubInfo.ConnectionId, conversation.ToUserHubInfo.ConnectionId})
                .OnMessageReceived(new ConversationChatMessageViewModel
                {
                    Message = sendMessageModel.Message,
                    SentAt = DateProvider.Get,
                    SentBy = currentUserId
                });


            _notificationService.OnConversationMessageSent(conversation, currentUserId);
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            string conversationId = Clients.CallerState.conversationId;
            if (conversationId != null)
            {
                var conversation = await _context.Conversations.Find(x => x.Id == conversationId).FirstOrDefaultAsync();

                if (conversation.FromUserId == Context.User.GetSubjectId())
                {
                    await _context.Conversations
                        .UpdateOneAsync(x => x.Id == conversationId, Builders<Conversation>.Update.Set(x => x.FromUserHubInfo.IsConnected, false));
                }
                else
                {
                    await _context.Conversations
                        .UpdateOneAsync(x => x.Id == conversationId, Builders<Conversation>.Update.Set(x => x.ToUserHubInfo.IsConnected, false));
                }
            }
        }

    }
}