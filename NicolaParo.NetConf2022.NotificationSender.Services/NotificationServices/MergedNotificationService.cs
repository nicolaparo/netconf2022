using NicolaParo.NetConf2022.NotificationSender.Models;

namespace NicolaParo.NetConf2022.NotificationSender.Services.NotificationServices
{
    public class MergedNotificationService : INotificationService
    {
        private readonly IEnumerable<INotificationService> notificationProviders;

        public MergedNotificationService(IEnumerable<INotificationService> notificationProviders)
        {
            this.notificationProviders = notificationProviders;
        }

        public bool CanSendNotification(Notification notification) => notificationProviders.Any(p => p.CanSendNotification(notification));

        public async Task SendNotificationAsync(Notification notification)
        {
            await Task.WhenAll(notificationProviders.Select(async p =>
            {
                if (p.CanSendNotification(notification))
                    await p.SendNotificationAsync(notification);
            }));
        }
    }
}