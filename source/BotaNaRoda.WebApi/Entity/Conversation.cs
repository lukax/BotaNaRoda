using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotaNaRoda.WebApi.Entity
{
    public class Conversation
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public User From { get; set; }
        public User To { get; set; }
        public Item Item { get; set; }
    }
}
