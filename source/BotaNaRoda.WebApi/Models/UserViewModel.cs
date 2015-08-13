using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotaNaRoda.WebApi.Domain;

namespace BotaNaRoda.WebApi.Models
{
    public class UserViewModel
    {
        public UserViewModel(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Avatar = user.Avatar;
        }

        public string Id { get; set; }

        public string Username { get; set; }

        public string Avatar { get; set; }
    }
}
