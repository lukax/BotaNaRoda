using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BotaNaRoda.WebApi.Domain
{
    public class UserRating
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string FromUserId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Message { get; set; }
        public int Score { get; set; }

        public UserRating()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}