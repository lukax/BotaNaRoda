using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Entity;

namespace BotaNaRoda.WebApi.Models
{
    public class PostItemBindingModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryType CategoryType { get; set; }

        public string[] ProductImages { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string CountryCode { get; set; }
        public string ZipCode { get; set; }
    }
}
