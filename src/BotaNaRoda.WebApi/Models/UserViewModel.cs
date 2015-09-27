using BotaNaRoda.WebApi.Entity;
using MongoDB.Driver.GeoJsonObjectModel;

namespace BotaNaRoda.WebApi.Models
{
    public class UserViewModel
    {
        public UserViewModel(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Avatar = user.Avatar;
            Name = user.Name;
            Locality = user.Locality;
        }

        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
        public string Name { get; set; }
        public string Locality { get; set; }
    }
}
