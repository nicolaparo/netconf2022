using NicolaParo.NetConf2022.NotificationSender.Services.NotificationServices;

namespace NicolaParo.NetConf2022.NotificationSender.Services
{
    public class NotificationServiceBuilder
    {
        private readonly List<INotificationService> notificationProviders = new();

        public NotificationServiceBuilder AddProvider(INotificationService notificationProvider)
        {
            notificationProviders.Add(notificationProvider);
            return this;
        }
        public NotificationServiceBuilder AddProvider<TNotificationProvider>() where TNotificationProvider : INotificationService, new()
        {
            return AddProvider(new TNotificationProvider());
        }

        public INotificationService Build() => new MergedNotificationService(notificationProviders);
    }

}