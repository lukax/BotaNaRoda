using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BotaNaRoda.Ndroid.Gcm
{
    public class ChatMessageNotification
    {
        public string Description { get; set; }
        public string ConversationId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
    }

    public class ItemNotification
    {
        public string Description { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
    }
}