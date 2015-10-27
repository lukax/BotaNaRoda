using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    public static class GeoUtil
    {
        public static Criteria GetFineCriteria()
        {
            var criteria = new Criteria();
            criteria.AltitudeRequired = false;
            criteria.BearingRequired = false;
            criteria.CostAllowed = false;
            criteria.PowerRequirement = Power.Low;
            criteria.Accuracy = Accuracy.Fine;
            return criteria;
        }

        public static Criteria GetCoarseCriteria()
        {
            var criteria = GetFineCriteria();
            criteria.Accuracy = Accuracy.Coarse;
            return criteria;
        }

        public static string DistanceTo(this ILatLon loc1, ILatLon loc2)
        {
			if ((loc1 == null || loc2 == null) ||
				(loc1.Latitude == 0 && loc1.Longitude == 0) || 
				(loc2.Latitude == 0 && loc2.Longitude == 0)) {
				return "";
			}
            return string.Format("{0:0,0.000}m", DistanceTo(loc1.Latitude, loc1.Longitude, loc2.Latitude, loc2.Longitude));
        }

        public static double DistanceTo(double lat1, double lon1, double lat2, double lon2, DistanceUnit unit = DistanceUnit.Kilometer)
        {
            double rlat1 = Math.PI * lat1 / 180;
            double rlat2 = Math.PI * lat2 / 180;
            double theta = lon1 - lon2;
            double rtheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(rlat1) * Math.Sin(rlat2) + Math.Cos(rlat1) *
                Math.Cos(rlat2) * Math.Cos(rtheta);
            dist = Math.Acos(dist);
            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            switch (unit)
            {
                case DistanceUnit.Kilometer: //Kilometers -> default
                    return dist * 1.609344;
                case DistanceUnit.Mile: //Miles
                    return dist;
            }

            return dist;
        }

        public enum DistanceUnit
        {
            Kilometer,
            Mile
        }

    }
}