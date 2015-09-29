using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotaNaRoda.WebApi.Entity
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
