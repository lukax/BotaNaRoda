using System;

namespace BotaNaRoda.Ndroid.Entity
{
    public class Item
    {
		public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime PostDate { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public Item()
        {
            Id = Guid.NewGuid().ToString();
        }
    }
}