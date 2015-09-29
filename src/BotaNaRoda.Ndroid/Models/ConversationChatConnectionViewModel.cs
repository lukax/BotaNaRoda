using System;
using System.Collections.Generic;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid
{
    internal class ConversationChatConnectionViewModel
    {
        public DateTime UpdatedAt { get; set; }
        public UserViewModel ToUser { get; set; }
        public IList<ConversationChatMessage> Messages { get; set; }
        public ItemListViewModel Item { get; set; }
    }
}