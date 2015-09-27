using System;

namespace BotaNaRoda.WebApi.Entity
{
    public interface IUpdatable
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }
}