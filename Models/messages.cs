namespace WebProject.Models
{
    public class messages
    {
            public int Id { get; set; }
            public string SenderId { get; set; }
            public string ReceiverId { get; set; }
            public string Content { get; set; }
            public DateTime Timestamp { get; set; }
        
    }
}
