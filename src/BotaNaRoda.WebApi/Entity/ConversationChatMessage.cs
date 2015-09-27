using System;
using BotaNaRoda.WebApi.Util;

namespace BotaNaRoda.WebApi.Entity
{
    public class ConversationChatMessage
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }

        public ConversationChatMessage()
        {
            SentAt = DateProvider.Get;
        }
    }
}