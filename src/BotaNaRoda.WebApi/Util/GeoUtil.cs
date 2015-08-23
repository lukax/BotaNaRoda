using System;

namespace BotaNaRoda.WebApi.Util
{
    public class GeoUtil
    {
        public enum DistanceUnit
        {
            Kilometer,
            Mile
        }

        static public double DistanceTo(double lat1, double lon1, double lat2, double lon2, DistanceUnit unit = DistanceUnit.Kilometer)
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
    }
}
