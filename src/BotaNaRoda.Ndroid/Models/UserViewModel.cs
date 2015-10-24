using Android.Widget;

namespace BotaNaRoda.Ndroid.Models
{
    public class UserViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string Locality { get; set; }
        public int DonationsCount { get; set; }
    }
}
