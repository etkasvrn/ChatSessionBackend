namespace ChatSessionBackend.Models
{
    public class ChatSession
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public Guid? AssignedAgentId { get; set; }
        public ChatStatus Status { get; set; } = ChatStatus.Pending;
        public int PollCount { get; set; } = 0;
        public DateTime LastPolled { get; set; } = DateTime.UtcNow;
    }

    
}
