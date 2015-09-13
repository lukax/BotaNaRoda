namespace BotaNaRoda.WebApi.Hubs
{
    public class SendConversationMessageBindingModel
    {
        public string ConversationId { get; set; }
        public string Message { get; set; }
    }
}