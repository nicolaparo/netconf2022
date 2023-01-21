namespace NicolaParo.NetConf2022.NotificationSender.App.Components.Charts.Models
{
    public record ChartItem
    {
        public string Label { get; set; }
        public float Value { get; set; }
        public string HtmlColor { get; set; }
    }
}
