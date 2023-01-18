using Microsoft.AspNetCore.Http.HttpResults;
using NicolaParo.NetConf2022.NotificationSender.Configuration;
using NicolaParo.NetConf2022.NotificationSender.Services;

namespace NicolaParo.NetConf2022.NotificationSender.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configurationModel = await ConfigurationModel.LoadFromFileAsync(@"..\..\_secrets\netConf2022\secrets.json");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add businss repositories and workers
            builder.Services.AddRepositories(configurationModel);
            builder.Services.AddBackgroundServices(configurationModel);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapEndpoints();

            app.Services.GetRequiredService<TelegramContactReceiver>().StartSerivce();
            app.Services.GetRequiredService<NotificationSchedulerService>().StartService();

            app.Run();

            await app.Services.GetRequiredService<TelegramContactReceiver>().StopServiceAsync();
            await app.Services.GetRequiredService<NotificationSchedulerService>().StopServiceAsync();
        }
    }
}