namespace Authorization.Domain.ConfigurationClasses
{
    public class SendGridConfiguration : ISendGridConfiguration
    {
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string ApiKey { get; set; }
    }
}
