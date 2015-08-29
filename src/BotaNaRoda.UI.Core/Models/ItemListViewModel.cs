﻿using System;

namespace BotaNaRoda.UI.Core.Models
{
    public class ItemListViewModel : ILocation
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Name { get; set; }
        public ImageInfo ThumbImage { get; set; }
        public ItemStatus Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
