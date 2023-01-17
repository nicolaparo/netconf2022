using NicolaParo.NetConf2022.NotificationSender.Models;
using NicolaParo.NetConf2022.NotificationSender.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NicolaParo.NetConf2022.NotificationSender.Services
{
    public class NotificationSchedulerService : IAsyncDisposable
    {
        private readonly INotificationService notificationService;
        private readonly IScheduledNotificationsRepository scheduledNotificationsRepository;
        private Task backgroundWorker;
        private CancellationTokenSource cancellationTokenSource;

        public NotificationSchedulerService(INotificationService notificationService, IScheduledNotificationsRepository scheduledNotificationsRepository)
        {
            this.notificationService = notificationService;
            this.scheduledNotificationsRepository = scheduledNotificationsRepository;
        }

        public void StartService()
        {
            if (!IsRunning)
            {
                cancellationTokenSource = new();
                backgroundWorker = Task.Run(() => DoWorkAsync(cancellationTokenSource.Token));
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
        public bool IsRunning => backgroundWorker is not null && cancellationTokenSource is not null;

        private async Task DoWorkAsync(CancellationToken cancellationToken)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

            var currentNotifications = new List<Notification>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var scheduledNotifications = await scheduledNotificationsRepository.GetScheduledNotificationsToSendAsync();

                foreach(var scheduledNotification in scheduledNotifications)
                {
                    try
                    {
                        await notificationService.SendNotificationAsync(scheduledNotification.Notification);
                        scheduledNotification.ActuallySentAt = DateTime.UtcNow;
                        await scheduledNotificationsRepository.UpdateScheduledNotificationAsync(scheduledNotification.Id, scheduledNotification);
                    }
                    catch(Exception ex) 
                    {

                    }
                }

                await timer.WaitForNextTickAsync(cancellationToken);
            }
        }

        public async ValueTask DisposeAsync()
        {
            await StopServiceAsync();
        }
    }
}
