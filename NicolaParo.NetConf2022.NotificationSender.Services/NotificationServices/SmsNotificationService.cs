using NicolaParo.NetConf2022.NotificationSender.Models;

namespace NicolaParo.NetConf2022.NotificationSender.Services.NotificationServices
{
    public class SmsNotificationService : INotificationService
    {
        public bool CanSendNotification(Notification notification) => notification.Contact.Phone is not null;

        public async Task SendNotificationAsync(Notification notification)
        {
            Console.WriteLine($"Sending sms notification to {notification.Contact.Phone.Number} ({notification.Contact?.FirstName} {notification.Contact?.LastName})...");
            await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(100, 1000)));
            Console.WriteLine($"Sms sent!");
        }
    }
}