using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Domain;
using BotaNaRoda.WebApi.Util;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Models
{
    public class ItemDetailViewModel
    {
        public ItemDetailViewModel(Item item, UserViewModel userViewModel)
        {
            Id = item.Id;
            User = userViewModel;
            CreatedAt = item.CreatedAt;
            Name = item.Name;
            Description = item.Description;
            Category = item.Category;
            Images = item.Images;
            ThumbImage = item.ThumbImage;
            Status = item.Status;
            Latitude = item.Loc.Latitude;
            Longitude = item.Loc.Longitude;
            Address = item.Address;
        }

        public string Id { get; set; }
        public UserViewModel User { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType Category { get; set; }

        public string[] Images { get; set; }
        public string ThumbImage { get; set; }

        public ItemStatus Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
    }
}
