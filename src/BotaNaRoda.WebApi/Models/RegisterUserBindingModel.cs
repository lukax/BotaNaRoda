namespace BotaNaRoda.WebApi.Models
{
    public class RegisterUserBindingModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Avatar { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string Locality { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }
    }
}
