namespace BotaNaRoda.WebApi.Domain
{
    public interface ILocalization
    {
        string Address { get; set; }
        string City { get; set; }
        string CountryCode { get; set; }
        string ZipCode { get; set; }

        double Latitude { get; set; }
        double Longitude { get; set; }
    }
}