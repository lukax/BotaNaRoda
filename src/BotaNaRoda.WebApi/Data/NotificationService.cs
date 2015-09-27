using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;
using Microsoft.Framework.OptionsModel;
using MongoDB.Bson;
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

        public void OnItemReservation(Item item)
        {
            string registrationId = "";
            AndroidPushBroker.QueueNotification(new GcmNotification().ForDeviceRegistrationId(registrationId)
                .WithJson(new
                {
                    
                }.ToJson()));
        }

        public void OnItemReservationCancelled(Item item)
        {
        }

        public void OnItemReceived(Item item)
        {
        }

        public void OnItemDelete(Item item)
        {
        }

        public void OnConversationMessageSent(Conversation conversation)
        {
        }

        private PushBroker _androidPushBroker;
        private PushBroker AndroidPushBroker
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
