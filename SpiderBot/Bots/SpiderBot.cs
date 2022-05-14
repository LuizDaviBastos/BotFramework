using BotScheduler.Dialogs;
using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler
{

    public class SpiderBot : ActivityHandler
    {
        private readonly MainDialog _mainDialog;
        private readonly UserState _userState;
        private readonly ConversationState _conversationState;
        private readonly IStatePropertyAccessor<DateTime> _lastAccessedTimeProperty;
        private readonly IStatePropertyAccessor<DialogState> _dialogStateProperty;
        private readonly IConfiguration _configuration;
        private int _expireAfterSeconds;

        public SpiderBot(MainDialog mainDialog, UserState userState, ConversationState conversationState, IConfiguration configuration)
        {
            _configuration = configuration;
            _conversationState = conversationState;
            _userState = userState;
            _mainDialog = mainDialog;

            _expireAfterSeconds = configuration.GetValue<int>("ExpireAfterSeconds");
            _lastAccessedTimeProperty = _conversationState.CreateProperty<DateTime>(nameof(_lastAccessedTimeProperty));
            _dialogStateProperty = _conversationState.CreateProperty<DialogState>(nameof(DialogState));
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await DialogGo.ToSendWelcomeActivity(turnContext, cancellationToken);
                }
            }
        }

        public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
        {
            var lastAccess = await _lastAccessedTimeProperty.GetAsync(turnContext, () => DateTime.UtcNow, cancellationToken).ConfigureAwait(false);
            if ((DateTime.UtcNow - lastAccess) >= TimeSpan.FromSeconds(_expireAfterSeconds))
            {
                await _conversationState.ClearStateAsync(turnContext, cancellationToken).ConfigureAwait(false);

                await DialogGo.ToSendWelcomeActivity(turnContext, cancellationToken);
            }

            await base.OnTurnAsync(turnContext, cancellationToken);

            //Save states of turn
            await _conversationState.SaveChangesAsync(turnContext);
            await _userState.SaveChangesAsync(turnContext);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            //receiver messages main. run the main dialog
            turnContext.Activity.Value = DialogIds.Initial;
            await _mainDialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(DialogIds.DialogState), cancellationToken);

            var conversationStateAccessors = _conversationState.CreateProperty<ConversationData>(nameof(ConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new ConversationData());

            if (string.IsNullOrEmpty(conversationData.ChannelId))
            {
                conversationData.ChannelId = turnContext.Activity.ChannelId.ToString();
            }
        }
    }
}