using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Entity
{
    public interface ILocalization
    {
        string Address { get; set; }
        string City { get; set; }
        string CountryCode { get; set; }
        string ZipCode { get; set; }

        GeoJson2DGeographicCoordinates Loc { get; set; }
    }
}