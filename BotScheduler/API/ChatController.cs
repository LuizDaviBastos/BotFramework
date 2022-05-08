using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotScheduler.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly ChatContext context;

        public ChatController(ChatContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<List<Model.ChatMessage>> ChatMessagesGetAsync(int sessionId)
        {
            var chat = await context.Chats.OrderByDescending(_ => _.Date).FirstOrDefaultAsync(_ => _.SessionId == sessionId);
            if (chat == null)
                return null;

            return await context.ChatMessages.Where(_ => _.ChannelId == chat.Id).ToListAsync();
        }
    }
}
