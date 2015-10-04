using System;
using BotaNaRoda.WebApi.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BotaNaRoda.WebApi.Entity
{
    public class ConversationChatMessage
    {
        public string Message { get; set; }
        public DateTime SentAt { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string SentBy { get; set; }

        public ConversationChatMessage()
        {
            SentAt = DateProvider.Get;
        }
    }
}