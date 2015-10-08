using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Controllers;
using BotaNaRoda.WebApi.Entity;
using Microsoft.Framework.Logging;
using Microsoft.Framework.OptionsModel;
using MongoDB.Bson;
using MongoDB.Driver;
using PushSharp;
using PushSharp.Android;
using PushSharp.Core;

namespace BotaNaRoda.WebApi.Data
{

    public class NotificationService : IDisposable
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ItemsContext _itemsContext;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(IOptions<AppSettings> appSettings, ItemsContext itemsContext, ILogger<NotificationService> logger)
        {
            _appSettings = appSettings;
            _itemsContext = itemsContext;
            _logger = logger;
        }

        public async void OnItemPromise(Item item)
        {
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

            var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = "O produto foi reservado!"
                };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        public async void OnItemPromiseDenied(Item item)
        {
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

            var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = "A reserva para o produto foi cancelada"
                };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        public async void OnItemReceived(Item item)
        {
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

            var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = "Doação concluída!"
                };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        public async void OnItemDelete(Item item)
        {
            var subscribers = await _itemsContext.Users.Find(x => item.Subscribers.Contains(x.Id)).ToListAsync();

            var notification = new ItemNotification
            {
                itemId = item.Id,
                itemName = item.Name,
                message = "O produto foi removido"
            };
            subscribers.ForEach(async x => await PostNotificationToUser(notification, x));
        }

        public async Task OnConversationMessageSent(Conversation conversation, string currentUserId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == conversation.ItemId).FirstAsync();

            var receivingEndUserId = conversation.FromUserId == currentUserId
                ? conversation.ToUserId
                : conversation.FromUserId;
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == receivingEndUserId).FirstAsync();

            var notification = new ChatMessageNotification
            {
                itemId = item.Id,
                itemName = item.Name,
                conversationId = conversation.Id,
                message = "Mensagem recebida",
            };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        public async Task OnItemPost(Item item, string userId)
        {
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();

            var notification = new ItemNotification
            {
                itemId = item.Id,
                itemName = item.Name,
                message = "Seu produto " + item.Name + " foi postado!"
            };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        public async Task OnItemSubscribe(Item item, string userId)
        {
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();

            var notification = new ItemNotification
            {
                itemId = item.Id,
                itemName = item.Name,
                message = receivingEndUser.Name + " está de olho!"
            };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        public async Task OnItemUnsubscribe(Item item, string userId)
        {
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();

            var notification = new ItemNotification
            {
                itemId = item.Id,
                itemName = item.Name,
                message = receivingEndUser.Name + " desistiu"
            };
            await PostNotificationToUser(notification, receivingEndUser);
        }

        private async Task<HttpClient> PostNotificationToUser(IDeviceNotification messageObj, User user)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + _appSettings.Options.AndroidApiKey);
            var response = await httpClient.PostAsync(new Uri("https://gcm-http.googleapis.com/gcm/send"), new StringContent(
             new
             {
                 to = user.PushDeviceRegistrationId,
                 data = messageObj
             }.ToJson(), Encoding.Default, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(string.Format("Could not send notification to {0}. ", user.Email), response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                _logger.LogInformation("Notification sent. " + response.Content.ReadAsStringAsync().Result);
            }
            return httpClient;
        }

        private async Task<HttpClient> PostNotificationToGlobally(IDeviceNotification messageObj)
        {
            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + _appSettings.Options.AndroidApiKey);
            var response = await httpClient.PostAsync(new Uri("https://gcm-http.googleapis.com/gcm/send"), new StringContent(
             new
             {
                 to = "/topics/global",
                 data = messageObj
             }.ToJson(), Encoding.Default, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Could not send notification globally. ", response.Content.ReadAsStringAsync().Result);
            }
            return httpClient;
        }

        public void Dispose()
        {
            //Stop and wait for the queues to drains before it dispose 
        }

    }
}
