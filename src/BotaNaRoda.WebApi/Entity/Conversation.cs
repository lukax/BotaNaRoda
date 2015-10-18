using System;
using System.Collections;
using System.Collections.Generic;
using BotaNaRoda.WebApi.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BotaNaRoda.WebApi.Entity
{
    [BsonIgnoreExtraElements]
    public class Conversation : IUpdatable
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

        public List<ConversationChatMessage> Messages { get; set; }

        [BsonIgnoreIfNull]
        public ConversationHubInfo HubInfo { get; set; }

        public Conversation()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateProvider.Get;
            Messages = new List<ConversationChatMessage>();
        }
    }

    public class ConversationHubInfo
    {
        public bool FromUserIsConnected { get; set; }
        public string FromUserConnectionId { get; set; }
        public bool ToUserIsConnected { get; set; }
        public string ToUserConnectionId { get; set; }
    }
}
