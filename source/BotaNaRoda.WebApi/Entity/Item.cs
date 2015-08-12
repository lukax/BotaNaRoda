using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BotaNaRoda.WebApi.Entity
{
    public class Item : ILocalization
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }

        public string[] ProductImages { get; set; }
        public string ImageThumb { get; set; }

        public bool Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }

        public Item()
        {
            Id = ObjectId.GenerateNewId().ToString();
            CreatedAt = DateTime.UtcNow;
        }

        public Item(PostItemBindingModel model) : this()
        {
            Name = model.Name;
            Description = model.Description;
            CategoryType = model.CategoryType;
            ProductImages = model.ProductImages;
            Latitude = model.Latitude;
            Longitude = model.Longitude;
            Address = model.Address;
            City = model.City;
            CountryCode = model.CountryCode;
            ZipCode = model.ZipCode;
        }
    }
}
