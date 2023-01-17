using NicolaParo.NetConf2022.NotificationSender.Models;

namespace NicolaParo.NetConf2022.NotificationSender.Services.NotificationServices
{
    public class EmailNotificationService : INotificationService
    {
        public bool CanSendNotification(Notification notification) => notification.Contact.Email is not null;

        public async Task SendNotificationAsync(Notification notification)
        {
            Console.WriteLine($"Sending email notification to {notification.Contact.Email.Address} ({notification.Contact?.FirstName} {notification.Contact?.LastName})...");
            await Task.Delay(TimeSpan.FromMilliseconds(Random.Shared.Next(100, 1000)));
            Console.WriteLine($"Mail sent!");
        }
    }
}