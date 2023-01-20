using Microsoft.AspNetCore.Mvc;
using NicolaParo.NetConf2022.NotificationSender.Api.Dtos;
using NicolaParo.NetConf2022.NotificationSender.Configuration;
using NicolaParo.NetConf2022.NotificationSender.Models;
using NicolaParo.NetConf2022.NotificationSender.Services;
using NicolaParo.NetConf2022.NotificationSender.Services.NotificationServices;
using NicolaParo.NetConf2022.NotificationSender.Services.Repositories;
using Telegram.Bot.Types;

namespace NicolaParo.NetConf2022.NotificationSender.Api
{
    public static class InitializationPipelineExtensions
    {
        public static void AddRepositories(this IServiceCollection services, ConfigurationModel configurationModel)
        {
            services.AddSingleton<NotificationSenderContext>(ctx => new NotificationSenderContext(configurationModel.DataFilePath));
            services.AddSingleton<IContactsRepository, ContactsRepository>();
            services.AddSingleton<IScheduledNotificationsRepository, ScheduledNotificationsRepository>();
        }
        public static void AddBackgroundServices(this IServiceCollection services, ConfigurationModel configurationModel)
        {
            services.AddSingleton<TelegramContactReceiver>(ctx =>
            {
                var service = new TelegramContactReceiver(
                    configuration: new TelegramContactReceiverConfiguration { Token = configurationModel.TelegramBotToken },
                    contactsRepository: ctx.GetRequiredService<IContactsRepository>()
                );

                return service;
            });
            services.AddSingleton<INotificationService>(ctx =>
            {
                var builder = new NotificationServiceBuilder()
                    .AddProvider(new TelegramNotificationService(
                        configuration: new TelegramNotificationServiceConfiguration { Token = configurationModel.TelegramBotToken }
                    ))
                    .AddProvider(new EmailNotificationService())
                    .AddProvider(new SmsNotificationService());

                var service = builder.Build();

                return service;
            });
            services.AddSingleton<NotificationSchedulerService>();
        }
        private static void MapContactsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var contacts = endpoints.MapGroup("/api/contacts");
            contacts.MapGet("", async (IContactsRepository cr) =>
            {
                var result = await cr.GetContactsAsync();
                return Results.Ok(result);
            });
            contacts.MapGet("{id}", async (IContactsRepository cr, Guid id) =>
            {
                var result = await cr.GetContactByIdAsync(id);
                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            });
            contacts.MapPost("", async (IContactsRepository cr, [FromBody] ContactInfo request) =>
            {
                if (request is null)
                    return Results.BadRequest("Invalid request");

                var result = await cr.CreateContactAsync(request);
                return Results.Ok(result);
            });
            contacts.MapPut("{id}", async (IContactsRepository cr, Guid id, [FromBody] ContactInfo request) =>
            {
                if (request is null)
                    return Results.BadRequest("Invalid request");

                var result = await cr.UpdateContactAsync(id, request);
                if (result)
                    return Results.Ok();

                return Results.NotFound();
            });
            contacts.MapDelete("{id}", async (IContactsRepository cr, Guid id) =>
            {
                var result = await cr.DeleteContactAsync(id);
                if (result)
                    return Results.Ok();

                return Results.NotFound();
            });
        }
        private static void MapNotificationsEndpoints(this IEndpointRouteBuilder endpoints)
        {
            var scheduledNotifications = endpoints.MapGroup("/api/notifications");
            scheduledNotifications.MapGet("", async (IScheduledNotificationsRepository cr, DateTime? fromDate, DateTime? toDate) =>
            {
                var result = await cr.GetScheduledNotificationsAsync(); //  n => (!fromDate.HasValue || n.ScheduleSendAt >= fromDate) && (!toDate.HasValue || n.ScheduleSendAt < toDate));
                return Results.Ok(result);
            });
            scheduledNotifications.MapGet("{id}", async (IScheduledNotificationsRepository cr, Guid id) =>
            {
                var result = await cr.GetScheduledNotificationByIdAsync(id);
                if (result is null)
                    return Results.NotFound();

                return Results.Ok(result);
            });
            scheduledNotifications.MapPost("/send-to-all-telegram", async (IScheduledNotificationsRepository cr, IContactsRepository cont, [FromBody] UpsertScheduledNotificationRequestDto request) =>
            {
                if (request is null)
                    return Results.BadRequest("Invalid request");

                var contacts = await cont.GetContactsAsync(c => c.Telegram != null);

                foreach(var contact in contacts)
                {
                    var scheduledNotfication = new ScheduledNotificationInfo
                    {
                        Notification = new Notification
                        {
                            Text = request.Text,
                            Title = request.Title,
                            Contact = contact
                        },
                        ScheduleSendAt = request.SendAt
                    };

                    await cr.InsertScheduledNotificationAsync(scheduledNotfication);
                }

                return Results.Ok();
            });
            scheduledNotifications.MapPost("", async (IScheduledNotificationsRepository cr, IContactsRepository cont, [FromBody] UpsertScheduledNotificationRequestForContactDto request) =>
            {
                if (request is null)
                    return Results.BadRequest("Invalid request");

                var contact = await cont.GetContactByIdAsync(request.ContactId);
                if (contact is null)
                    return Results.BadRequest("Invalid ContactId");

                var scheduledNotfication = new ScheduledNotificationInfo
                {
                    Notification = new Notification
                    {
                        Text = request.Text,
                        Title = request.Title,
                        Contact = contact
                    },
                    ScheduleSendAt = request.SendAt
                };

                var result = await cr.InsertScheduledNotificationAsync(scheduledNotfication);

                return Results.Ok(result);
            });
            scheduledNotifications.MapPut("{id}", async (IScheduledNotificationsRepository cr, IContactsRepository cont, Guid id, [FromBody] UpsertScheduledNotificationRequestForContactDto request) =>
            {
                if (request is null)
                    return Results.BadRequest("Invalid request");

                var contact = await cont.GetContactByIdAsync(request.ContactId);
                if (contact is null)
                    return Results.BadRequest("Invalid ContactId");

                var scheduledNotfication = await cr.GetScheduledNotificationByIdAsync(id);
                if (scheduledNotfication is null)
                    return Results.NotFound();

                scheduledNotfication.Notification = new Notification
                {
                    Contact = contact,
                    Text = request.Text,
                    Title = request.Title
                };

                var result = await cr.UpdateScheduledNotificationAsync(id, scheduledNotfication);
                if (result)
                    return Results.Ok();

                return Results.NotFound();
            });
            scheduledNotifications.MapDelete("{id}", async (IScheduledNotificationsRepository cr, Guid id) =>
            {
                var result = await cr.DeleteScheduledNotificationAsync(id);
                if (result)
                    return Results.Ok();

                return Results.NotFound();
            });

        }
        public static void MapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapContactsEndpoints();
            endpoints.MapNotificationsEndpoints();
        }
    }
}