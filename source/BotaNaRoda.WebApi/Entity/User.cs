using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotaNaRoda.WebApi.Entity
{
    public class User : ILocalization
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public string Username { get; set; }
        public string Avatar { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }
    }
}
