using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NicolaParo.NetConf2022.NotificationSender.Api;
using NicolaParo.NetConf2022.NotificationSender.Configuration;
using NicolaParo.NetConf2022.NotificationSender.Services;

public class Program
{

    public static async Task Main(string[] args)
    {
        var configurationModel = await ConfigurationModel.LoadFromFileAsync(@"..\..\_secrets\netConf2022\secrets.json");

        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Add businss repositories and workers
        builder.Services.AddRepositories(configurationModel);
        builder.Services.AddBackgroundServices(configurationModel);

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHsts();

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapEndpoints();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Services.GetRequiredService<TelegramContactReceiver>().StartSerivce();
        app.Services.GetRequiredService<NotificationSchedulerService>().StartService();

        app.Run();

        await app.Services.GetRequiredService<TelegramContactReceiver>().StopServiceAsync();
        await app.Services.GetRequiredService<NotificationSchedulerService>().StopServiceAsync();
    }
}
