using System;

namespace BotaNaRoda.Ndroid.Models
{
    public class ItemDetailViewModel
    {
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
