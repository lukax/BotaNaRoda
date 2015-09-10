using System;

namespace BotaNaRoda.Ndroid.Models
{
    public class ItemDetailViewModel : ILocation
    {
        public string Id { get; set; }
        public UserDetailViewModel User { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType Category { get; set; }
        public ImageInfo[] Images { get; set; }
        public ImageInfo ThumbImage { get; set; }
        public ItemStatus Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
    }
}
