using System;
using System.Collections.Generic;
using System.Linq;
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
using BotaNaRoda.Ndroid.Models;
using Newtonsoft.Json;

namespace BotaNaRoda.Ndroid.Controllers.Services
{
    [Service(Exported = false)]
    class PostItemService : IntentService
    {
        public const string ItemExtra = "item";
        private readonly ItemRestService _itemsService;
        private UserRepository _userRepository;

        // Create the IntentService, name the worker thread for debugging purposes:
        public PostItemService() : base("PostItemService")
        {
            _userRepository = new UserRepository();
            _itemsService = new ItemRestService(_userRepository);
        }

        // OnHandleIntent is invoked on a worker thread:
        protected override void OnHandleIntent(Intent intent)
        {
            var item = JsonConvert.DeserializeObject<ItemCreateBindingModel>(intent.GetStringExtra(ItemExtra));
            try
            {
                Log.Info("PostItemService", "Posting item");

                var loadingNotificationId = 0;
                SendLoadingNotification(item, loadingNotificationId);

                var itemId = _itemsService.PostItem(item).Result;

                var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
                notificationManager.Cancel(loadingNotificationId);

                if (itemId != null)
                {
                    SendItemPostedNotification(item, 1, JsonConvert.DeserializeObject<string>(itemId));
                }
                else
                {
                    SendItemErrorNotification(item, 2);
                }
            }
            catch (Exception)
            {
                Log.Debug("PostItemService", "Failed to post item");
                SendItemErrorNotification(item, 2);
            }
        }

        private void SendItemErrorNotification(ItemCreateBindingModel item, int notificationId)
        {
            Intent mainIntent = new Intent(this, typeof(MainActivity));
            mainIntent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, mainIntent, PendingIntentFlags.CancelCurrent);

            var notificationBuilder = new Notification.Builder(this)
                .SetDefaults(NotificationDefaults.Sound | NotificationDefaults.Lights)
                .SetPriority((int)NotificationPriority.Max)
                .SetSmallIcon(Resource.Drawable.ic_gps)
                .SetContentTitle("Bota Na Roda")
                .SetContentText("Não foi possível colocar produto " + item.Name + " na roda.")
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(notificationId, notificationBuilder.Build());
        }

        private void SendLoadingNotification(ItemCreateBindingModel item, int notificationId)
        {
            Intent mainIntent = new Intent(this, typeof(MainActivity));
            mainIntent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, mainIntent, PendingIntentFlags.CancelCurrent);

            var notificationBuilder = new Notification.Builder(this)
                .SetDefaults(NotificationDefaults.Lights)
                .SetPriority((int)NotificationPriority.High)
                .SetSmallIcon(Resource.Drawable.ic_gps)
                .SetContentTitle("Bota Na Roda")
                .SetContentText("Colocando " + item.Name + " na roda...")
                .SetProgress(100, 50, true)
                .SetContentIntent(pendingIntent);
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(notificationId, notificationBuilder.Build());
        }

        private void SendItemPostedNotification(ItemCreateBindingModel item, int notificationId, string itemId)
        {
            Intent itemDetailIntent = new Intent(this, typeof(ItemDetailActivity));
            itemDetailIntent.PutExtra(ItemDetailActivity.ItemIdExtra, itemId);
            itemDetailIntent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, itemDetailIntent, PendingIntentFlags.CancelCurrent);

            var notificationBuilder = new Notification.Builder(this)
                .SetDefaults(NotificationDefaults.All)
                .SetPriority((int)NotificationPriority.Max)
                .SetSmallIcon(Resource.Drawable.ic_gps)
                .SetContentTitle("Bota Na Roda")
                .SetContentText(item.Name + " está na roda!")
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);
            var notificationManager = (NotificationManager)GetSystemService(Context.NotificationService);
            notificationManager.Notify(notificationId, notificationBuilder.Build());
        }
    }
}