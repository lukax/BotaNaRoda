using System;
using System.Collections.Generic;
using BotaNaRoda.WebApi.Entity;

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
            ReservedByMe = item.ReservedBy == userViewModel.Id;
        }

        public string Id { get; set; }
        public UserViewModel User { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType Category { get; set; }

        public ICollection<ImageInfo> Images { get; set; }
        public ImageInfo ThumbImage { get; set; }

        public ItemStatus Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }

        public bool ReservedByMe { get; set; }
    }
}
