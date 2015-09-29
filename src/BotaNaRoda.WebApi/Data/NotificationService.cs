using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;
using Microsoft.Framework.OptionsModel;
using MongoDB.Bson;
using MongoDB.Driver;
using PushSharp;
using PushSharp.Android;

namespace BotaNaRoda.WebApi.Data
{

    public class NotificationService : IDisposable
    {
        private readonly IOptions<AppSettings> _appSettings;
        private readonly ItemsContext _itemsContext;

        public NotificationService(IOptions<AppSettings> appSettings, ItemsContext itemsContext)
        {
            _appSettings = appSettings;
            _itemsContext = itemsContext;
        }

        public async void OnItemReservation(Item item)
        {
            var usr = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

            AndroidPushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(usr.PushDeviceRegistrationId)
                .WithJson(new ItemNotification
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Description = "O produto foi reservado!"
                }.ToJson()));
        }

        public async void OnItemReservationCancelled(Item item)
        {
            var usr = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

            AndroidPushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(usr.PushDeviceRegistrationId)
                .WithJson(new ItemNotification
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Description = "A reserva para o produto foi cancelada"
                }.ToJson()));
        }

        public async void OnItemReceived(Item item)
        {
            var usr = await _itemsContext.Users.Find(x => x.Id == item.UserId).FirstAsync();

            AndroidPushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(usr.PushDeviceRegistrationId)
                .WithJson(new ItemNotification
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Description = "Doação concluída!"
                }.ToJson()));
        }

        public async void OnItemDelete(Item item)
        {
            var subscribers = await _itemsContext.Users.Find(x => item.Subscribers.Contains(x.Id)).ToListAsync();

            AndroidPushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(subscribers.Select(x=> x.PushDeviceRegistrationId))
                .WithJson(new ItemNotification
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    Description = "O produto foi removido"
                }.ToJson()));
        }

        public async void OnConversationMessageSent(Conversation conversation, string receivingEndUserId)
        {
            var item = await _itemsContext.Items.Find(x => x.Id == conversation.ItemId).FirstAsync();
            var receivingEndUser = await _itemsContext.Users.Find(x => x.Id == receivingEndUserId).FirstAsync();
            
            AndroidPushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(receivingEndUser.PushDeviceRegistrationId)
                .WithJson(new ChatMessageNotification
                {
                    ItemId = item.Id,
                    ItemName = item.Name,
                    ConversationId = conversation.Id,
                    Description = "Mensagem recebida",
                }.ToJson()));
        }

        private PushBroker _androidPushBroker;
        public PushBroker AndroidPushBroker
        {
            get
            {
                if (_androidPushBroker == null)
                {
                    _androidPushBroker = new PushBroker();
                    _androidPushBroker.RegisterGcmService(new GcmPushChannelSettings(_appSettings.Options.AndroidApiKey));
                }
                return _androidPushBroker;
            }
        }


        public void Dispose()
        {
            //Stop and wait for the queues to drains before it dispose 
            _androidPushBroker?.StopAllServices();
        }
    }
}
