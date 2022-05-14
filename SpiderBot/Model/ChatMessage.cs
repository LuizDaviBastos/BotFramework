using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotScheduler.Model
{
    public class ChatMessage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public string Message { get; set; }
        public string Creator { get; set; }
        public string ChannelId { get; set; }
    }
}
