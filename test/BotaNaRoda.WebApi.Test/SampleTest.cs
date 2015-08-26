using System;
using System.Collections.Generic;
using System.Linq;
using BotaNaRoda.WebApi.Models;
using Microsoft.AspNet.Http;
using MongoDB.Bson;
using Xunit;

namespace BotaNaRoda.WebApi.Test
{
    public class SampleTest
    {
        [Fact]
        public void Test1()
        {
            var user = new RegisterUserBindingModel
            {
                Username = "lucas",
                Address = "123 st",
                Locality = "Niteroi",
                CountryCode = "BR",
                Email = "espdlucas@gmail.com",
                Latitude = -22.9068467,
                Longitude = -43.1728965,
                Password = "passw0rd",
                PostalCode = "123456-789",
                Avatar = ""
            };
            Console.WriteLine(user.ToJson());
        }
    }
}
