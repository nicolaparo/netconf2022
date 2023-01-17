namespace NicolaParo.NetConf2022.NotificationSender.Models
{
    public record Notification
    {
        public Contact Contact { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
    }
}