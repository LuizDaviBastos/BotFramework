using Microsoft.Bot.Builder;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BotScheduler
{
    public class RepositoryService
    {
        private ChatContext context;

        public RepositoryService(ChatContext context)
        {
            this.context = context;
        }

        public async Task<bool> ChatAddAsync(ITurnContext turnContext)
        {
            var chat = context.Chats.FirstOrDefault(_ => _.Id == turnContext.Activity.Conversation.Id);
            if (chat == null)
            {
                chat = new Model.Chat { Id = turnContext.Activity.Conversation.Id, Date = DateTime.Now };
                context.Chats.Add(chat);
                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> MessageAddAsync(string message, string creator, ITurnContext turnContext)
        {
            context.ChatMessages.Add(new Model.ChatMessage { Message = message, Creator = creator, DateTime = DateTime.Now, ChannelId = turnContext.Activity.Conversation.Id });
            await context.SaveChangesAsync();
            return true;
        }

        internal async Task<bool> ChatUpdateSessionAsync(int sessionId, ITurnContext turnContext)
        {
            var chat = context.Chats.FirstOrDefault(_ => _.Id == turnContext.Activity.Conversation.Id);
            if (chat != null)
            {
                chat.SessionId = sessionId;
                await context.SaveChangesAsync();
            }

            return true;
        }
    }
}
