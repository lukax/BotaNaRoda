using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Domain;

namespace BotaNaRoda.WebApi.Models
{
    public class ListItemViewModel
    {
        public ListItemViewModel(Item item, UserViewModel userViewModel)
        {
            Id = item.Id;
            User = userViewModel;
            CreatedAt = item.CreatedAt;
            Name = item.Name;
            Description = item.Description;
            CategoryType = item.CategoryType;
            ProductImages = item.ProductImages;
            ImageThumb = item.ImageThumb;
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
        public CategoryType CategoryType { get; set; }

        public string[] ProductImages { get; set; }
        public string ImageThumb { get; set; }

        public bool Status { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }

        public double Distance { get; set; }
    }
}
