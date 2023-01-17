using System.Text.Json;
using System.Text.Json.Serialization;

namespace NicolaParo.NetConf2022.NotificationSender.Configuration
{
    public record ConfigurationModel
    {
        public static async Task<ConfigurationModel> LoadFromFileAsync(string filepath)
        {
            var secrets = await File.ReadAllTextAsync(filepath);
            return JsonSerializer.Deserialize<ConfigurationModel>(secrets);
        }

        [JsonPropertyName("telegramBotToken")]
        public string TelegramBotToken { get; set; }

        [JsonPropertyName("scheduledNotificationsDataFilePath")]
        public string ScheduledNotificationsDataFilePath { get; set; }

        [JsonPropertyName("contactsDataFilePath")]
        public string ContactsDataFilePath { get; set; }

        [JsonPropertyName("dataFilePath")]
        public string DataFilePath { get; set; }
    }
}