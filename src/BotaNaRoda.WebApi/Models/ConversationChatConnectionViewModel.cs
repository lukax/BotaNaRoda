using System;
using System.Collections.Generic;
using BotaNaRoda.WebApi.Entity;

namespace BotaNaRoda.WebApi.Models
{
    public class ConversationChatConnectionViewModel 
    {
        public DateTime UpdatedAt { get; set; }
        public UserViewModel ToUser { get; set; }
        public IList<ConversationChatMessage> Messages { get; set; }
        public ItemListViewModel Item { get; set; }
    }
}