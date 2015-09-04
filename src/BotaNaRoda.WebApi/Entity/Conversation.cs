using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BotaNaRoda.WebApi.Entity
{
    public class Conversation
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ItemId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string FromUserId { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string ToUserId { get; set; }

        public ICollection<ConversationMessage> Messages { get; set; }

        public Conversation()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
            Messages = new HashSet<ConversationMessage>();
        }
    }

    public class ConversationMessage
    {
        public string Message { get; set; }
        public DateTime Sent { get; set; }
    }
}
