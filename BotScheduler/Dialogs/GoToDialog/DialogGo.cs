using BotScheduler.Library.Keys;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs.GoToDialog
{
    public class DialogGo
    {
        public static async Task<DialogTurnResult> ToMainDialog(WaterfallStepContext waterfallStep, CancellationToken cancellationToken, string message = "")
        {
            await waterfallStep.Context.SendActivityAsync(MessageFactory.Text(message));
            waterfallStep.Context.Activity.Value = DialogIds.Secondary;
            return await waterfallStep.ReplaceDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
        }

        public static async Task<DialogTurnResult> ToCreateAppointment(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("Ok, let's create a new appointment"));
            waterfallStep.Context.Activity.Value = DialogIds.Secondary;
            return await waterfallStep.ReplaceDialogAsync(DialogIds.CreateAppointmentDialog, null, cancellationToken);
        }

        public static async Task<DialogTurnResult> ToGetAppointment(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            //await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("Lets's verify if you have an appointment"));
            waterfallStep.Context.Activity.Value = DialogIds.Secondary;
            return await waterfallStep.ReplaceDialogAsync(DialogIds.GetAppointmentDialog, null, cancellationToken);
        }

        public static async Task<DialogTurnResult> ToDontUnderstand(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("Hmm.. I don't understand, try again"), cancellationToken: cancellationToken);
            waterfallStep.Context.Activity.Value = DialogIds.Secondary;
            return await waterfallStep.ReplaceDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
        }
        public static async Task<DialogTurnResult> ToCancelDialog(DialogContext innerDc, CancellationToken cancellationToken)
        {
            await innerDc.Context.SendActivityAsync(MessageFactory.Text("you canceled this dialogs"), cancellationToken);
            await innerDc.CancelAllDialogsAsync(cancellationToken);
            innerDc.Context.Activity.Value = DialogIds.Secondary;
            return await innerDc.ReplaceDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
        }

        public static async Task ToSendWelcomeActivity(ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Attachment(GetWelcomeCard()), cancellationToken);
        }

        public static async Task ToSendWelcomeActivity(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Attachment(GetWelcomeCard()), cancellationToken);
        }

        private static Attachment GetWelcomeCard()
        {
            var listActions = new List<CardAction>()
            {
                new CardAction(){ Title = "Create a Announcement", Type = ActionTypes.PostBack,  Value = Intents.CreateAnnouncement},
            };

            var card = new HeroCard()
            {
                Title = "Hello and welcome!",
                Text = "Your options:",
                Buttons = listActions
            };

            return card.ToAttachment();
        }
    }
}
