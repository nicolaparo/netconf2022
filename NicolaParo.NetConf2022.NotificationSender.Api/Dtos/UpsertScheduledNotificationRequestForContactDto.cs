namespace NicolaParo.NetConf2022.NotificationSender.Api.Dtos
{
    public record UpsertScheduledNotificationRequestForContactDto
    {
        public Guid ContactId { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public DateTime SendAt { get; set; }
    }
}