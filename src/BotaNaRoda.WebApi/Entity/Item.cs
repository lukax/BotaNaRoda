using System;
using System.Collections.Generic;
using System.Linq;
using BotaNaRoda.WebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Entity
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

        public ICollection<ImageInfo> Images { get; set; }
        public ImageInfo ThumbImage { get; set; }

        public ItemStatus Status { get; set; }

        public GeoJson2DGeographicCoordinates Loc { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }

        public Item()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
            Images = new HashSet<ImageInfo>();
        }

        public Item(ItemCreateBindingModel model, string userId) 
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
            Name = model.Name;
            UserId = userId;
            Description = model.Description;
            Category = model.Category;
            Images = model.Images;
            ThumbImage = model.ThumbImage;
            Loc = GeoJson.Geographic(model.Longitude, model.Latitude);
            Address = model.Address;
            Locality = model.Locality;
            CountryCode = model.CountryCode;
            PostalCode = model.PostalCode;
        }
    }
}
