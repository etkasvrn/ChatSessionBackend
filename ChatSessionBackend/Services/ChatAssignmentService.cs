namespace ChatSessionBackend.Services
{
    public class ChatAssignmentService
    {
        private readonly IAgentService _agentService;
        private readonly IQueueService _queueService;

        public ChatAssignmentService(IAgentService agentService, IQueueService queueService)
        {
            _agentService = agentService;
            _queueService = queueService;
        }

        public void AssignChats()
        {
            while (_queueService.HasChats())
            {
                var chatSession = _queueService.Dequeue();
                var agent = _agentService.AssignAgentToChat(chatSession);

                if (agent == null)
                {
                    // Handle case where no agent could be assigned
                }
            }
        }
    }


}
