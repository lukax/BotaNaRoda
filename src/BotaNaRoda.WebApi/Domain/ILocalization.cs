using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Domain
{
    public interface ILocalization
    {
        string Address { get; set; }
        string City { get; set; }
        string CountryCode { get; set; }
        string ZipCode { get; set; }

        GeoJson2DGeographicCoordinates Coordinates { get; set; }
    }
}