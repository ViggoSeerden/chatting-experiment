namespace chatexperiment.Models
{
    public class Message
    {
        public int UserId { get; set;  }
    
        public string? Content { get; set; }
        
        public DateTime Timestamp { get; set; }
    }
}    