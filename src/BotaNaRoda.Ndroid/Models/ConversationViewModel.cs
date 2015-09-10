using System;
using System.Collections.Generic;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid
{
    public class ConversationViewModel
    {
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ItemDetailViewModel Item { get; set; }

        public UserDetailViewModel ToUser { get; set; }

        public ICollection<ConversationMessage> Messages { get; set; }
    }

    public class ConversationMessage
    {
        public string Message { get; set; }
        public DateTime Sent { get; set; }
    }
}