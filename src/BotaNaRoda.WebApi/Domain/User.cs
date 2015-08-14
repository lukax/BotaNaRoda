using System;
using System.Collections.Generic;
using BotaNaRoda.WebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Domain
{
    public class User : ILocalization
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public string Avatar { get; set; }

        public GeoJson2DGeographicCoordinates Loc { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }

        public int Credits { get; set; }
        public List<UserRating> Ratings { get; set; }

        public User()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
        }

        public User(RegisterUserBindingModel model)
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
            Username = model.Username;
            Avatar = model.Avatar;
            Loc = GeoJson.Geographic(model.Longitude, model.Latitude);
            Address = model.Address;
            City = model.City;
            CountryCode = model.CountryCode;
            ZipCode = model.ZipCode;
        }
    }
}
