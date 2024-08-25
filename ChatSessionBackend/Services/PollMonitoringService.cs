using ChatSessionBackend.Models;

namespace ChatSessionBackend.Services
{
    public interface IPollMonitoringService
    {
        void MonitorPolls();
    }

    public class PollMonitoringService : IPollMonitoringService
    {
        private readonly IQueueService _queueService;

        public PollMonitoringService(IQueueService queueService)
        {
            _queueService = queueService;
        }

        public void MonitorPolls()
        {
            foreach (var chat in _queueService.GetActiveChats())
            {
                var timeSinceLastPoll = DateTime.UtcNow - chat.LastPolled;
                Console.WriteLine($"[MONITOR] {DateTime.UtcNow}: Monitoring chat session {chat.Id}. Time since last poll: {timeSinceLastPoll.TotalSeconds} seconds");


                if (timeSinceLastPoll.TotalSeconds > 30000)
                {
                    chat.Status = ChatStatus.Inactive;
                    Console.WriteLine($"[MONITOR] {DateTime.UtcNow}: Chat session {chat.Id} marked as inactive due to inactivity.");

                }
            }
        }
    }
}
