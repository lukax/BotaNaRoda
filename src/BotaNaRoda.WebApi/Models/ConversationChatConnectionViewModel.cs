using System;
using System.Collections.Generic;
using BotaNaRoda.WebApi.Entity;

namespace BotaNaRoda.WebApi.Models
{
    public class ConversationChatConnectionViewModel 
    {
        public DateTime UpdatedAt { get; set; }
        public UserViewModel ToUser { get; set; }
        public IList<ConversationChatMessageViewModel> Messages { get; set; }
        public ItemListViewModel Item { get; set; }
    }

    public class ConversationChatMessageViewModel
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }
        public string SentBy { get; set; }
    }
}