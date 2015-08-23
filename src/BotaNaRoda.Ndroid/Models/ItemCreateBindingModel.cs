namespace BotaNaRoda.Ndroid.Models
{
    public class ItemCreateBindingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType Category { get; set; }

        public string[] ProductImages { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }
    }
}
