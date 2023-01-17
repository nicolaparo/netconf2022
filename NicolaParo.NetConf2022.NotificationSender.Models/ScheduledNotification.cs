namespace NicolaParo.NetConf2022.NotificationSender.Models
{
    public record ScheduledNotificationInfo
    {
        public Notification Notification { get; set; }
        public DateTime ScheduleSendAt { get; set; }
        public DateTime? ActuallySentAt { get; set; }
    }
    public record ScheduledNotification : ScheduledNotificationInfo
    {
        public ScheduledNotification() { }

        public ScheduledNotification(ScheduledNotificationInfo info) : base(info) { }

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}