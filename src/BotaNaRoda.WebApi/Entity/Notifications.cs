using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace BotaNaRoda.WebApi.Entity
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public interface IDeviceNotification
    {
        string message { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ChatMessageNotification : IDeviceNotification
    {
        public string message { get; set; }
        public string conversationId { get; set; }
        public string itemId { get; set; }
        public string itemName { get; set; }
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public class ItemNotification : IDeviceNotification
    {
        public string message { get; set; }
        public string itemId { get; set; }
        public string itemName { get; set; }
    }
}
