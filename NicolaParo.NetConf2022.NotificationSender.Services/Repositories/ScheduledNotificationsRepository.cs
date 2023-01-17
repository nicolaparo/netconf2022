using LiteDB;
using NicolaParo.NetConf2022.NotificationSender.Models;
using System;
using System.Linq.Expressions;
using System.Text.Json;

namespace NicolaParo.NetConf2022.NotificationSender.Services.Repositories
{
    public interface IScheduledNotificationsRepository
    {
        Task<Guid> InsertScheduledNotificationAsync(ScheduledNotificationInfo contactData);
        Task<bool> DeleteScheduledNotificationAsync(Guid id);
        Task<ScheduledNotification> GetScheduledNotificationByIdAsync(Guid id);
        Task<ScheduledNotification[]> GetScheduledNotificationsAsync();
        Task<ScheduledNotification[]> GetScheduledNotificationsAsync(Expression<Func<ScheduledNotification, bool>> predicate, int skip = 0, int? take = null);
        Task<bool> UpdateScheduledNotificationAsync(Guid id, ScheduledNotificationInfo contactData);

        event Func<ScheduledNotification, Task> OnScheduledNotificationInserted;
        event Func<ScheduledNotification, Task> OnScheduledNotificationUpdated;
        event Func<ScheduledNotification, Task> OnScheduledNotificationDeleted;
    }

    public class ScheduledNotificationsRepository : IScheduledNotificationsRepository
    {
        private readonly string databaseFileName;

        public ScheduledNotificationsRepository(string scheduledNotificationsFileName)
        {
            this.databaseFileName = scheduledNotificationsFileName;
        }

        private NotificationSenderContext CreateContext() => new NotificationSenderContext(databaseFileName);

        public Task<ScheduledNotification[]> GetScheduledNotificationsAsync(Expression<Func<ScheduledNotification, bool>> predicate, int skip = 0, int? take = null)
        {
            using var context = CreateContext();

            var scheduledNotificationsQuery = context.ScheduledNotifications.Query();

            if (predicate != null)
                scheduledNotificationsQuery = scheduledNotificationsQuery.Where(predicate);

            scheduledNotificationsQuery = scheduledNotificationsQuery.OrderBy(x => x.ScheduleSendAt);

            var scheduledNotificationsQueryResult = (ILiteQueryableResult<ScheduledNotification>)scheduledNotificationsQuery;

            if (skip > 0)
                scheduledNotificationsQueryResult = scheduledNotificationsQueryResult.Skip(skip);

            if (take is not null)
                scheduledNotificationsQueryResult = scheduledNotificationsQueryResult.Limit(take.Value);
                
            var scheduledNotifications = scheduledNotificationsQueryResult.ToArray();

            return Task.FromResult(scheduledNotifications);
        }
        public Task<ScheduledNotification[]> GetScheduledNotificationsAsync()
        {
            using var context = CreateContext();

            var scheduledNotifications = context.ScheduledNotifications.FindAll().ToArray();

            return Task.FromResult(scheduledNotifications);
        }
        public Task<ScheduledNotification> GetScheduledNotificationByIdAsync(Guid id)
        {
            using var context = CreateContext();

            var scheduledNotification = context.ScheduledNotifications.FindOne(sn => sn.Id == id);

            return Task.FromResult(scheduledNotification);
        }
        public async Task<Guid> InsertScheduledNotificationAsync(ScheduledNotificationInfo scheduledNotificationData)
        {
            using var context = CreateContext();

            var scheduledNotification = new ScheduledNotification(scheduledNotificationData);

            context.ScheduledNotifications.Insert(scheduledNotification);

            if (OnScheduledNotificationInserted is not null)
                await OnScheduledNotificationInserted(scheduledNotification);

            return scheduledNotification.Id;
        }
        public async Task<bool> UpdateScheduledNotificationAsync(Guid id, ScheduledNotificationInfo scheduledNotificationData)
        {
            using var context = CreateContext();

            var scheduledNotification = context.ScheduledNotifications.FindOne(sn => sn.Id == id);
            if (scheduledNotification is null)
                return false;

            scheduledNotification = new ScheduledNotification(scheduledNotificationData) with { Id = scheduledNotification.Id, UpdatedAt = DateTime.UtcNow };

            context.ScheduledNotifications.Update(scheduledNotification);

            if (OnScheduledNotificationUpdated is not null)
                await OnScheduledNotificationUpdated(scheduledNotification);

            return true;
        }
        public async Task<bool> DeleteScheduledNotificationAsync(Guid id)
        {
            using var context = CreateContext();

            var scheduledNotification = context.ScheduledNotifications.FindOne(sn => sn.Id == id);
            if (scheduledNotification is null)
                return false;

            context.ScheduledNotifications.Delete(id);

            if (OnScheduledNotificationDeleted is not null)
                await OnScheduledNotificationDeleted(scheduledNotification);

            return true;
        }

        public event Func<ScheduledNotification, Task> OnScheduledNotificationInserted;
        public event Func<ScheduledNotification, Task> OnScheduledNotificationUpdated;
        public event Func<ScheduledNotification, Task> OnScheduledNotificationDeleted;
    }

    public static class ScheduledNotificationsRepositoryExtensions
    {
        public static Task<ScheduledNotification[]> GetScheduledNotificationsToSendAtAsync(this IScheduledNotificationsRepository repository, DateTime sendAt)
        {
            return repository.GetScheduledNotificationsAsync(n => n.ActuallySentAt == null && n.ScheduleSendAt < sendAt);
        }
        public static Task<ScheduledNotification[]> GetScheduledNotificationsToSendAsync(this IScheduledNotificationsRepository repository)
        {
            return repository.GetScheduledNotificationsToSendAtAsync(DateTime.UtcNow);
        }
    }
}