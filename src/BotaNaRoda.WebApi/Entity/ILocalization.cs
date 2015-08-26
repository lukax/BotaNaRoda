using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Entity
{
    public interface ILocalization
    {
        string Address { get; set; }
        string Locality { get; set; }
        string CountryCode { get; set; }
        string PostalCode { get; set; }

        GeoJson2DGeographicCoordinates Loc { get; set; }
    }
}