using System;
using System.Collections.Generic;
using BotaNaRoda.WebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Entity
{
    public class User : ILocalization
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }

        public string Name { get; set; }
        public string Avatar { get; set; }

        public GeoJson2DGeographicCoordinates Loc { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }

        public int Credits { get; set; }
        public ICollection<UserReview> Reviews { get; set; }

        public string Provider { get; set; }
        public string ProviderId { get; set; }

        public User()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
            Reviews = new HashSet<UserReview>();
        }

        public User(RegisterUserBindingModel model)
            :this()
        {
            Username = model.Username;
            Avatar = model.Avatar;
            Loc = GeoJson.Geographic(model.Longitude, model.Latitude);
            Address = model.Address;
            Locality = model.Locality;
            CountryCode = model.CountryCode;
            PostalCode = model.PostalCode;
        }
    }
}
