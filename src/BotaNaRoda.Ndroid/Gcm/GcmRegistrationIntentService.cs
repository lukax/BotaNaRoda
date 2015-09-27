using System;
using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Gms.Gcm.Iid;
using Android.Util;
using BotaNaRoda.Ndroid.Data;

namespace BotaNaRoda.Ndroid.Gcm
{
    [Service(Exported = false)]
    public class GcmRegistrationIntentService : IntentService
    {
        static readonly object Locker = new object();
        private readonly ItemRestService _itemsService;
        private bool _tokenSent = false;
        private UserRepository _userRepository;

        public GcmRegistrationIntentService() : base("RegistrationIntentService")
        {
            _userRepository = new UserRepository();
            _itemsService = new ItemRestService(_userRepository);
        }

        protected override void OnHandleIntent(Intent intent)
        {
            try
            {
                Log.Info("RegistrationIntentService", "Calling InstanceID.GetToken");
                lock (Locker)
                {
                    var instanceId = InstanceID.GetInstance(this);
                    var token = instanceId.GetToken(GetString(Resource.String.gcm_defaultSenderId), GoogleCloudMessaging.InstanceIdScope, null);

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
            if (!_tokenSent && _userRepository.IsLoggedIn)
            {
                _tokenSent = _itemsService.PostDeviceRegistrationId(token).Result;
            }
        }

        void Subscribe(string token)
        {
            var pubSub = GcmPubSub.GetInstance(this);
            pubSub.Subscribe(token, "/topics/global", null);
        }
    }
}