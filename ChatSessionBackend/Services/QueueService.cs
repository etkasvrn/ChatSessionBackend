using ChatSessionBackend.Helpers;
using ChatSessionBackend.Models;

namespace ChatSessionBackend.Services
{
    public interface IQueueService
    {
        Result Enqueue(ChatSession chatSession);
        Result Poll(Guid chatId);
        ChatSession Dequeue();
        bool HasChats();
        IEnumerable<ChatSession> GetActiveChats();
    }

    public class QueueService : IQueueService
    {
        private readonly Queue<ChatSession> _chatQueue = new Queue<ChatSession>();
        private readonly int _maxQueueLength;

        public QueueService(int maxQueueLength)
        {
            _maxQueueLength = maxQueueLength;
        }

        public Result Enqueue(ChatSession chatSession)
        {
            if (_chatQueue.Count >= _maxQueueLength)
            {
                return Result.Fail("Queue is full.");
            }
            Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Chat session {chatSession.Id} added to queue.");
            _chatQueue.Enqueue(chatSession);
            Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Chat session {chatSession.Id} added to queue.");
            return Result.Success();
        }

        public ChatSession Dequeue()
        {

            if (_chatQueue.Count > 0)
            {
                var dequeuedSession = _chatQueue.Dequeue();
                Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Chat session {dequeuedSession.Id} dequeued. Remaining queue count: {_chatQueue.Count}");
                return dequeuedSession;
            }
            return null;
        }


        //public ChatSession Peek()
        //{
        //    if (_chatQueue.Count > 0)
        //    {
        //        var peekedSession = _chatQueue.Peek();
        //        Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Chat session {peekedSession.Id} peeked. Queue count remains: {_chatQueue.Count}");
        //        return peekedSession;
        //    }
        //    return null;
        //}

        public bool HasChats()
        {
            return _chatQueue.Count > 0;
        }

        public Result Poll(Guid chatId)
        {
            Console.WriteLine("[DEBUG] Current chat sessions in queue:");
            foreach (var session in _chatQueue)
            {
                Console.WriteLine($"Session ID: {session.Id}, Last Polled: {session.LastPolled}");
            }

            var chatSession = _chatQueue.FirstOrDefault(chat => chat.Id == chatId);
            if (chatSession == null)
            {
                Console.WriteLine($"[ERROR] {DateTime.UtcNow}: Chat session {chatId} not found.");
                return Result.Fail("Chat session not found.");
            }
            var timeSinceLastPoll = DateTime.UtcNow - chatSession.LastPolled;
            Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Polling chat session {chatId}. Time since last poll: {timeSinceLastPoll.TotalSeconds} seconds");


            if (timeSinceLastPoll.TotalSeconds > 30000)
            {
                chatSession.Status = ChatStatus.Inactive;
                Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Chat session {chatId} marked as inactive due to inactivity.");
                return Result.Fail("Chat session inactive due to inactivity.");
            }

            chatSession.LastPolled = DateTime.UtcNow;
            Console.WriteLine($"[DEBUG] {DateTime.UtcNow}: Chat session {chatId} polled successfully. Last polled time updated to {chatSession.LastPolled}.");

            return Result.Success();
        }

        public IEnumerable<ChatSession> GetActiveChats()
        {
            return _chatQueue.Where(chat => chat.Status == ChatStatus.Active);
        }
    }

}
