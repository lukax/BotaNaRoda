namespace BotaNaRoda.WebApi.Models
{
    public class UserLocalizationBindingModel 
    {
        public string Address { get; set; }
        public string Locality { get; set; }
        public string CountryCode { get; set; }
        public string PostalCode { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}