using System;
using BotaNaRoda.WebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Domain
{
    public class Item : ILocalization
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType Category { get; set; }

        public string[] Images { get; set; }
        public string ThumbImage { get; set; }

        public ItemStatus Status { get; set; }

        public GeoJson2DGeographicCoordinates Loc { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }

        public Item()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
        }

        public Item(PostItemBindingModel model, string userId) : this()
        {
            Name = model.Name;
            UserId = userId;
            Description = model.Description;
            Category = model.CategoryType;
            Images = model.ProductImages;
            Loc = GeoJson.Geographic(model.Longitude, model.Latitude);
            Address = model.Address;
            City = model.City;
            CountryCode = model.CountryCode;
            ZipCode = model.ZipCode;
        }
    }
}
