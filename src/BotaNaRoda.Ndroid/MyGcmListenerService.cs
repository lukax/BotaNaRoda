using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Gms.Gcm;
using Android.Graphics;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Controllers;

namespace BotaNaRoda.Ndroid
{
	[Service (Exported = false), IntentFilter (new [] { "com.google.android.c2dm.intent.RECEIVE" })]
	public class MyGcmListenerService : GcmListenerService
	{
		public override void OnMessageReceived (string from, Bundle data)
		{
			// Extract the message received from GCM:
			var message = data.GetString("message");
			Log.Debug("MyGcmListenerService", "From:    " + from);
			Log.Debug("MyGcmListenerService", "Message: " + message);

			// Forward the received message in a local notification:
		    var conversationId = data.GetString("conversationId");
		    var itemId = data.GetString("itemId");

			SendNotification (message, conversationId, itemId);
		}

		// Use Notification Builder to create and launch the notification:
		void SendNotification (string message, string conversationId, string itemId)
		{
		    Intent intent;
		    if (conversationId != null)
		    {
		        intent = new Intent(this, typeof(ChatActivity));
		        intent.PutExtra(ChatActivity.ConversationIdExtra, conversationId);
		    }
            else if (itemId != null)
            {
                intent = new Intent(this, typeof(ItemDetailActivity));
                intent.PutExtra(ItemDetailActivity.ItemIdExtra, itemId);
            }
            else
            {
                intent = new Intent(this, typeof(MainActivity));
            }
			intent.AddFlags (ActivityFlags.ClearTop);
			var pendingIntent = PendingIntent.GetActivity (this, 0, intent, PendingIntentFlags.OneShot);

			var notificationBuilder = new Notification.Builder (this)
                .SetDefaults(NotificationDefaults.All)
                .SetPriority((int)NotificationPriority.Max)
                .SetSmallIcon (Resource.Drawable.ic_gps)
				.SetContentTitle ("Bota Na Roda")
				.SetContentText (message)
				.SetAutoCancel (true)
                .SetContentIntent (pendingIntent);

			var notificationManager = (NotificationManager)GetSystemService (Context.NotificationService);
			notificationManager.Notify (0, notificationBuilder.Build ());
		}
	}
}