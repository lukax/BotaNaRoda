using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Data;
using BotaNaRoda.WebApi.Entity;
using BotaNaRoda.WebApi.Models;
using IdentityServer3.Core.Extensions;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ItemsContext _itemsContext;
        private readonly ILogger<AccountController> _logger;

        public AccountController(ItemsContext itemsContext, ILogger<AccountController> logger)
        {
            _itemsContext = itemsContext;
            _logger = logger;
        }

        [HttpPost("Localization")]
        public async Task<IActionResult> PostUserLocalization(UserLocalizationBindingModel localization)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var result = await _itemsContext.Users.UpdateOneAsync(
                Builders<User>.Filter.Eq(x => x.Id, User.GetSubjectId()),
                Builders<User>.Update
                    .Set(x => x.Address, localization.Address)
                    .Set(x => x.Locality, localization.Locality)
                    .Set(x => x.CountryCode, localization.CountryCode)
                    .Set(x => x.PostalCode, localization.PostalCode)
                    .Set(x => x.Loc, new GeoJson2DGeographicCoordinates(localization.Longitude, localization.Latitude))
                );

            return new HttpOkResult();
        }

        [HttpPost("DeviceRegistrationId/{registrationId}")]
        public async Task<IActionResult> PostDeviceRegistrationId(string registrationId)
        {
            if (!ModelState.IsValid)
            {
                return HttpBadRequest(ModelState);
            }

            var result = await _itemsContext.Users.UpdateOneAsync(
                Builders<User>.Filter.Eq(x => x.Id, User.GetSubjectId()), 
                Builders<User>.Update.Set(x => x.PushDeviceRegistrationId, registrationId));

            return new HttpOkResult();
        }
    }
}
