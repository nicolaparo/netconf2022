using LiteDB;
using NicolaParo.NetConf2022.NotificationSender.Models;

namespace NicolaParo.NetConf2022.NotificationSender.Services.Repositories
{
    public class NotificationSenderContext : IDisposable
    {
        private readonly LiteDatabase database;

        public NotificationSenderContext(string databaseFileName)
        {
            database = new LiteDatabase(databaseFileName);

            Contacts.EnsureIndex(c => c.Id);
            ScheduledNotifications.EnsureIndex(sn => sn.Id);
            ScheduledNotifications.EnsureIndex(sn => sn.ScheduleSendAt);
        }

        public ILiteCollection<Contact> Contacts => database.GetCollection<Contact>(@"contacts");
        public ILiteCollection<ScheduledNotification> ScheduledNotifications => database.GetCollection<ScheduledNotification>(@"scheduledNotifications");

        public void Dispose()
        {
            database.Dispose();
        }
    }
}