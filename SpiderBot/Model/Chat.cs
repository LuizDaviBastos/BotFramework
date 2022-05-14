using System;

namespace BotScheduler.Model
{
    public class Chat
    {
        public string Id { get; set; }
        public DateTime Date { get; set; }
        public int SessionId { get; internal set; }
    }
}
