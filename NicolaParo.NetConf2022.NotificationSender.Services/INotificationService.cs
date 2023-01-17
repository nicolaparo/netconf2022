using NicolaParo.NetConf2022.NotificationSender.Models;

namespace NicolaParo.NetConf2022.NotificationSender.Services
{
    public interface INotificationService
    {
        public bool CanSendNotification(Notification notification);
        public Task SendNotificationAsync(Notification notification);
    }
}