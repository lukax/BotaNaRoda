namespace BotaNaRoda.WebApi
{
    public class AppSettings
    {
        public string AppName { get; set; }
        public string IdSvrAuthority { get; set; }
        public string IdSvrAudience { get; set; }
        public string BotaNaRodaDatabaseName { get; set; }
        public string BotaNaRodaConnectionString { get; set; }
        public string StorageConnectionString { get; set; }
        public string LogglyCustomerToken { get; set; }
    }
}