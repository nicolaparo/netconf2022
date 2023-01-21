using Bogus;
using NicolaParo.NetConf2022.NotificationSender.Configuration;
using NicolaParo.NetConf2022.NotificationSender.Models;
using NicolaParo.NetConf2022.NotificationSender.Services.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace NicolaParo.NetConf2022.NotificationSender.DataInitializer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Environment.CurrentDirectory = Path.Combine(Environment.CurrentDirectory, "../../..");

            var configurationModel = await ConfigurationModel.LoadFromFileAsync(@"..\..\_secrets\netConf2022\secrets.json");

            var context = new NotificationSenderContext(configurationModel.DataFilePath);

            var contactsRepository = new ContactsRepository(context);
            var scheduledNotifications = new ScheduledNotificationsRepository(context);

            int contactsToCreate = 100;
            int notificationsToCreate = 5000;

            for (var i = 0; i < contactsToCreate; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Creating contacts... {i * 100 / contactsToCreate}%");

                var fakeContact = CreateFakeContactInfo();

                await contactsRepository.CreateContactAsync(fakeContact);
            }

            var contacts = await contactsRepository.GetContactsAsync();

            for (var i = 0; i < notificationsToCreate; i++)
            {
                Console.SetCursorPosition(0, 0);
                Console.WriteLine($"Creating notifications... {i * 100 / notificationsToCreate}%");

                var contact = contacts.OrderBy(_ => Random.Shared.Next()).FirstOrDefault();

                var fakeNotification = CreateFakeScheduledNotificationInfo(contact);
                await scheduledNotifications.InsertScheduledNotificationAsync(fakeNotification);

            }

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Fake data created!");

        }

        private static ContactInfo CreateFakeContactInfo()
        {
            var faker = new Faker();

            var contact = new ContactInfo
            {
                FirstName = faker.Name.FirstName(),
                LastName = faker.Name.LastName(),
                Phone = new PhoneContactInfo
                {
                    Number = faker.Phone.PhoneNumber()
                }
            };

            contact.Email = new EmailContactInfo
            {
                Address = $"{contact.FirstName}.{contact.LastName}@{faker.Internet.DomainName()}".ToLower()
            };

            return contact;
        }
        private static ScheduledNotificationInfo CreateFakeScheduledNotificationInfo(Contact contact)
        {
            var faker = new Faker();

            var date = faker.Date.Between(DateTime.UtcNow.AddDays(-2), DateTime.UtcNow.AddDays(2));

            return new ScheduledNotificationInfo
            {
                Notification = new Notification
                {
                    Contact = contact,
                    Title = faker.Lorem.Sentence(),
                    Text = faker.Lorem.Paragraph()
                },
                ScheduleSendAt = date,
                ActuallySentAt = date < DateTime.UtcNow ? date : null
            };
        }
    }
}
