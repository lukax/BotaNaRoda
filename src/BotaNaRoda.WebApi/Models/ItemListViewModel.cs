using System;
using BotaNaRoda.WebApi.Entity;

namespace BotaNaRoda.WebApi.Models
{
    public class ItemListViewModel
    {
        public ItemListViewModel(Item item)
        {
            Id = item.Id;
            CreatedAt = item.CreatedAt;
            Name = item.Name;
            ThumbImage = item.ThumbImage;
            Status = item.Status;
            Latitude = item.Loc.Latitude;
            Longitude = item.Loc.Longitude;
        }

        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public ImageInfo ThumbImage { get; set; }
        public ItemStatus Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
