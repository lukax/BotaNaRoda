using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Util
{
    public class GeoUtil
    {
        public static string GetDistance(Location currentLocation, ILocation item)
        {
            if (currentLocation != null)
            {
                Location itemLocation = new Location(currentLocation)
                {
                    Latitude = item.Latitude,
                    Longitude = item.Longitude
                };
                return string.Format("{0:0,0.00}m", currentLocation.DistanceTo(itemLocation));
            }
            return "??";
        }
    }
}