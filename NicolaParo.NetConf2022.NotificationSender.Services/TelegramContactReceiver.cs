using NicolaParo.NetConf2022.NotificationSender.Models;
using NicolaParo.NetConf2022.NotificationSender.Services.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace NicolaParo.NetConf2022.NotificationSender.Services
{
    public class TelegramContactReceiver : IAsyncDisposable
    {
        private readonly TelegramBotClient telegramClient;
        private readonly IContactsRepository contactsRepository;
        private CancellationTokenSource cancellationTokenSource;
        private Task backgroundWorker;

        public TelegramContactReceiver(TelegramContactReceiverConfiguration configuration, IContactsRepository contactsRepository)
        {
            telegramClient = new TelegramBotClient(configuration.Token);
            this.contactsRepository = contactsRepository;
        }

        public void StartSerivce()
        {
            if (!IsRunning)
            {
                cancellationTokenSource = new();
                backgroundWorker = Task.Run(() => telegramClient.ReceiveAsync(OnUpdateReceived, OnException, cancellationToken: cancellationTokenSource.Token));
            }
        }
        public async Task StopServiceAsync()
        {
            if (IsRunning)
            {
                cancellationTokenSource.Cancel();
                await backgroundWorker;
                backgroundWorker = null;
            }
        }
        public bool IsRunning => cancellationTokenSource is not null && backgroundWorker is not null;

        public async Task OnUpdateReceived(ITelegramBotClient client, Update update, CancellationToken cancellationToken)
        {
            var chatId = update.Message.Chat.Id;

            Console.WriteLine(update.Message.Text);

            var contact = (await contactsRepository.GetContactsAsync(c => c.Telegram != null && c.Telegram.ChatId == chatId)).FirstOrDefault();

            if (contact is null)
            {
                var contactData = new ContactInfo()
                {
                    FirstName = update.Message.Chat.FirstName,
                    LastName = update.Message.Chat.LastName,
                    Telegram = new TelegramContactInfo()
                    {
                        ChatId = chatId,
                        Username = update.Message.Chat.Username,
                    }
                };

                await contactsRepository.CreateContactAsync(contactData);
            }
        }
        public async Task OnException(ITelegramBotClient client, Exception exception, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            await StopServiceAsync();
        }
    }

    public record TelegramContactReceiverConfiguration
    {
        public string Token { get; set; }
    }
}
