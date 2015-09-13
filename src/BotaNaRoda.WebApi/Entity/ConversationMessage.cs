using System;
using BotaNaRoda.WebApi.Util;

namespace BotaNaRoda.WebApi.Entity
{
    public class ConversationMessage
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }

        public ConversationMessage()
        {
            SentAt = DateProvider.Get;
        }
    }
}