namespace ChatSessionBackend.Models
{
    public class Team
    {
        public string Name { get; set; }
        public List<Agent> Agents { get; set; } = new List<Agent>();

        public int CalculateCapacity()
        {
            return Agents.Sum(agent => (int)Math.Floor(agent.MaxConcurrency * agent.GetEfficiency()));
        }
    }

    public static class AgentExtensions
    {
        public static double GetEfficiency(this Agent agent)
        {
            return agent.Seniority switch
            {
                SeniorityLevel.Junior => 0.4,
                SeniorityLevel.MidLevel => 0.6,
                SeniorityLevel.Senior => 0.8,
                SeniorityLevel.TeamLead => 0.5,
                _ => 0.4
            };
        }
    }
}
