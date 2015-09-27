using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;

namespace BotaNaRoda.WebApi.Models
{
    public class ConversationListViewModel
    {
        public string Id { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ToUserName { get; set; }
        public string ItemName { get; set; }
        public string ItemThumbImage { get; set; }
    }

    public class ConversationDetailViewModel
    {
        public string Id { get; set; }
        public DateTime LastUpdated { get; set; }
        public string ToUserName { get; set; }
        public string ToUserAvatar { get; set; }
        public string ItemName { get; set; }
        public string ItemThumbImage { get; set; }
        public double ItemLatitude { get; set; }
        public double ItemLongitude { get; set; }
        public List<ConversationMessage> Messages { get; set; }
    }
}
