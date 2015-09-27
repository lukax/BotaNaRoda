using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Data;
using ModernHttpClient;

namespace BotaNaRoda.Ndroid.Auth
{
    [Service(Exported = false)]
    public class GcmRegistrationIntentService : IntentService
    {
        static readonly object Locker = new object();
        private readonly ItemRestService _itemsService;

        public GcmRegistrationIntentService() : base("RegistrationIntentService")
        {
            _itemsService = new ItemRestService(new UserRepository());
        }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (Locker)
                {
                    var instanceID = InstanceID.GetInstance(this);
                    var token = instanceID.GetToken(
                        "YOUR_SERVER_ID", GoogleCloudMessaging.InstanceIdScope, null);

                    Log.Info("RegistrationIntentService", "GCM Registration Token: " + token);
                    SendRegistrationToAppServer(token);
                    Subscribe(token);
                }
            }
            catch (Exception e)
            {
                Log.Debug("RegistrationIntentService", "Failed to get a registration token");
                return;
            }
        }

        void SendRegistrationToAppServer(string token)
        {
            //TODO store info that has already been sent
            _itemsService.PostDeviceRegistrationId(token).Wait();
        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}