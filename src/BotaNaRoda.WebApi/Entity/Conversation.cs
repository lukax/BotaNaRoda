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

        [BsonIgnoreIfNull] public ConversationHubInfo ToUserHubInfo { get; set; }
        [BsonIgnoreIfNull] public ConversationHubInfo FromUserHubInfo { get; set; }

        public Conversation()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateProvider.Get;
            Messages = new List<ConversationChatMessage>();
            ToUserHubInfo = new ConversationHubInfo();
            FromUserHubInfo = new ConversationHubInfo();
        }

        public string GetReceivingEndUserId(string currentUserId)
        {
            return FromUserId == currentUserId ? ToUserId : FromUserId;
        }

        public ConversationHubInfo GetReceivingEndHubInfo(string currentUserId)
        {
            return FromUserId == currentUserId ? ToUserHubInfo : FromUserHubInfo;
        }
    }

    public class ConversationHubInfo
    {
        public bool IsConnected { get; set; }
        public string ConnectionId { get; set; }
    }
}
