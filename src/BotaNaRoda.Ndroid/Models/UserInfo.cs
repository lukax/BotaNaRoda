using BotaNaRoda.Ndroid.Models;

namespace BotaNaRoda.Ndroid.Data
{
    public class UserInfo : ILocation
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}