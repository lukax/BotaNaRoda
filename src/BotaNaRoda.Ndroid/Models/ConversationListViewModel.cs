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

namespace BotaNaRoda.Ndroid.Models
{
    public class ConversationListViewModel
    {
        public string Id { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ToUserName { get; set; }
        public string ItemName { get; set; }
        public string ItemThumbImage { get; set; }
    }

    public class ConversationDetailViewModel : ILatLon
    {
        public string Id { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ToUserName { get; set; }
        public string ToUserAvatar { get; set; }
        public string ItemName { get; set; }
        public string ItemThumbImage { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public List<ConversationChatMessage> Messages { get; set; }
    }

    public class ConversationChatMessage
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
		public string SentBy { get; set; }
    }
}