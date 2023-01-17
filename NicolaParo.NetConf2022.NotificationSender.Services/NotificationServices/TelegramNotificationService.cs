using NicolaParo.NetConf2022.NotificationSender.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace NicolaParo.NetConf2022.NotificationSender.Services.NotificationServices
{
    public record TelegramNotificationServiceConfiguration
    {
        public string Token { get; set; }
    }

    public class TelegramNotificationService : INotificationService
    {
        private readonly TelegramBotClient telegramClient;

        public TelegramNotificationService(TelegramNotificationServiceConfiguration configuration)
        {
            telegramClient = new TelegramBotClient(configuration.Token);
        }

        public bool CanSendNotification(Notification notification) => notification.Contact.Telegram is not null && ToChatId(notification.Contact.Telegram) is not null;

        public async Task SendNotificationAsync(Notification notification)
        {
            Console.WriteLine($"Sending telegram notification to chat {notification.Contact.Telegram.ChatId} ({notification.Contact?.FirstName} {notification.Contact?.LastName})...");

            var telegramContactInfo = notification.Contact.Telegram;
            var chatId = ToChatId(telegramContactInfo);

            await telegramClient.SendTextMessageAsync(chatId, $"{notification.Title}\r\n\r\n{notification.Text}");

            Console.WriteLine($"Message sent!");
        }

        private static ChatId ToChatId(TelegramContactInfo telegramContactInfo)
        {
            if (telegramContactInfo.ChatId.HasValue)
                return new ChatId(telegramContactInfo.ChatId.Value);
            return default;
        }
    }
}