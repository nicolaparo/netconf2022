namespace NicolaParo.NetConf2022.NotificationSender.Models
{
    public record Contact : ContactInfo
    {
        public Contact() { }
        public Contact(ContactInfo info) : base(info) { }

        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
    public record ContactInfo
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
        public EmailContactInfo Email { get; set; }
        public PhoneContactInfo Phone { get; set; }
        public TelegramContactInfo Telegram { get; set; }
    }
    public record EmailContactInfo
    {
        public string Address { get; set; }
    }
    public record PhoneContactInfo
    {
        public string Number { get; set; }
    }
    public record TelegramContactInfo
    {
        public string Username { get; set; }
        public long? ChatId { get; set; }
    }

}