namespace ChatSessionBackend.Models
{
    public class Agent
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public SeniorityLevel Seniority { get; set; }
        public  int CurrentChats { get; set; } = 0;
        public int MaxConcurrency { get; set; } = 10;
        public bool IsActive { get; set; } = true;

    }



}
