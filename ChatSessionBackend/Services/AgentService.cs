using ChatSessionBackend.Models;

namespace ChatSessionBackend.Services
{
    public interface IAgentService
    {
        Agent AssignAgentToChat(ChatSession chatSession);
    }

    public class AgentService : IAgentService
    {
        private readonly List<Team> _teams;

        public AgentService(List<Team> teams)
        {
            _teams = teams;
        }

        public Agent AssignAgentToChat(ChatSession chatSession)
        {
            foreach (var team in _teams)
            {
                var availableAgent = team.Agents
                    .Where(agent => agent.IsActive && agent.CurrentChats < agent.MaxConcurrency)
                    .OrderBy(agent => agent.Seniority)
                    .FirstOrDefault();

                if (availableAgent != null)
                {
                    availableAgent.CurrentChats++;
                    chatSession.AssignedAgentId = availableAgent.Id;
                    chatSession.Status = ChatStatus.Active;
                    return availableAgent;
                }
            }

            chatSession.Status = ChatStatus.Rejected;
            return null;
        }
    }


}
