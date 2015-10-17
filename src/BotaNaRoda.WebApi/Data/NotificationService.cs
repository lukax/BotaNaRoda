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

        public void OnItemPromise(Item item)
        {
            Task.Run(async () =>
            {
                var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

                var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = "O produto foi reservado!"
                };
                await PostNotificationToUser(notification, receivingEndUser);

            });
        }

        public void OnItemPromiseDenied(Item item)
        {
            Task.Run(async () =>
            {
                var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

                var notification = new ItemNotification
                    {
                        itemId = item.Id,
                        itemName = item.Name,
                        message = "A reserva para o produto foi cancelada"
                    };
                await PostNotificationToUser(notification, receivingEndUser);
            });
        }

        public void OnItemReceived(Item item)
        {
            Task.Run(async () =>
            {
                var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

                var notification = new ItemNotification
                    {
                        itemId = item.Id,
                        itemName = item.Name,
                        message = "Doação concluída!"
                    };
                await PostNotificationToUser(notification, receivingEndUser);
            });
        }

        public void OnItemDelete(Item item)
        {
            Task.Run(async () =>
            {
                var owner = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();
                var subscribers = await _itemsContext.Users.Find(x => item.Subscribers.Contains(x.Id)).ToListAsync();

                var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = "O produto " + item.Name + " foi excluído"
                };
                await PostNotificationToUser(notification, owner);
                subscribers.ForEach(async x => await PostNotificationToUser(notification, x));
            });
        }

        public void OnConversationMessageSent(Conversation conversation, string currentUserId)
        {
            Task.Run(async () =>
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
                    message = "Nova mensagem recebida",
                };
                await PostNotificationToUser(notification, receivingEndUser);
            });
        }

        public void OnItemSubscribe(Item item, string userId)
        {
            Task.Run(async () =>
            {
                var emittingUser = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();
                var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

                var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = emittingUser.Name + " está de olho em " + item.Name + "!"
                };
                await PostNotificationToUser(notification, receivingEndUser);
            });
        }

        public void OnItemUnsubscribe(Item item, string userId)
        {
            Task.Run(async () =>
            {
                var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == userId).FirstAsync();

                var notification = new ItemNotification
                {
                    itemId = item.Id,
                    itemName = item.Name,
                    message = receivingEndUser.Name + " não está mais de olho em " + item.Name
                };
                await PostNotificationToUser(notification, receivingEndUser);
            });
        }

        private async Task<bool> PostNotificationToUser(IDeviceNotification messageObj, User user)
        {
            HttpClient httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(30)};
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + _appSettings.Options.AndroidApiKey);
            try
            {
                var response =  await httpClient.PostAsync(new Uri("https://gcm-http.googleapis.com/gcm/send"), new StringContent(
                        new
                        {
                            to = user.PushDeviceRegistrationId,
                            data = messageObj
                        }.ToJson(), Encoding.Default, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"Could not send notification to {user.Email}. ", response.Content.ReadAsStringAsync().Result);
                }
                else
                {
                    _logger.LogInformation("Notification sent. " + response.Content.ReadAsStringAsync().Result);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not send notification", ex));
            }
            return false;
        }

        private async Task<bool> PostNotificationToGlobally(IDeviceNotification messageObj)
        {
            HttpClient httpClient = new HttpClient {Timeout = TimeSpan.FromSeconds(30)};
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "key=" + _appSettings.Options.AndroidApiKey);
            try
            {
                var response =
                    await httpClient.PostAsync(new Uri("https://gcm-http.googleapis.com/gcm/send"), new StringContent(
                        new
                        {
                            to = "/topics/global",
                            data = messageObj
                        }.ToJson(), Encoding.Default, "application/json"));

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Could not send notification globally. ", response.Content.ReadAsStringAsync().Result);
                }

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(string.Format("Could not send notification", ex));
            }
            return false;
        }

        public void Dispose()
        {
            //Stop and wait for the queues to drains before it dispose 
        }

    }
}
