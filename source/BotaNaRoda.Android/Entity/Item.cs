using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace BotaNaRoda.Android.Entity
{
    public class Item
    {
		public string Id { get; set; }
        public string Description { get; set; }
        public DateTime PostDate { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }

        public Item()
        {
        }
    }
}