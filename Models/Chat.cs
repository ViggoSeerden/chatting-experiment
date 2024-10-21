namespace chatexperiment.Models
{
    public class Chat
    {
        public int ChatId { get; set; }

        public int TenantId { get; set; }

        public int LandlordId { get; set; }

        public List<Message> Messages { get; set; } = new List<Message>();
    }
}