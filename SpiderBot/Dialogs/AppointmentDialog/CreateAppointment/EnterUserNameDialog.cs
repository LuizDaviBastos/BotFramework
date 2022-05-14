using BotScheduler.Library;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs.CreateAppointment
{
    public class EnterUserNameDialog : ComponentDialog
    {
        private readonly LuisRecognizerQuery _luisRecognizerQuery;
        private readonly RepositoryService repositoryService;

        public EnterUserNameDialog(string dialogId, LuisRecognizerQuery luisRecognizerQuery, RepositoryService repositoryService) : base(dialogId)
        {
            this._luisRecognizerQuery = luisRecognizerQuery;
            this.repositoryService = repositoryService;
            var waterfallDialogs = new WaterfallStep[]
            {
                GetUserNameAsync,
                QuestionUserNameAsync,
                LoopUserNameAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallDialogs));
            AddDialog(new TextPrompt($"{DialogIds.TextPrompt}UserName"));
            AddDialog(new TextPrompt($"{DialogIds.TextPrompt}QuestionUserName"));
            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> GetUserNameAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["UserProfile"] = waterfallStep.Options;

            var message = "Please, enter your full name";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync($"{DialogIds.TextPrompt}UserName", new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> QuestionUserNameAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var fullName = waterfallStep.Result;
            (waterfallStep.Values["UserProfile"] as UserProfile).Name = (string)fullName;

            var message = $"Confirm your name \"{fullName}\"?";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync($"{DialogIds.TextPrompt}QuestionUserName", new PromptOptions()
            { Prompt = MessageFactory.Text(message) }, cancellationToken);
        }

        public async Task<DialogTurnResult> LoopUserNameAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var intent = await _luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);

            switch (intent.GetTopScoringIntent().intent)
            {
                case Intents.UtilitiesConfirm:
                    return await waterfallStep.EndDialogAsync(waterfallStep.Values["UserProfile"], cancellationToken);

                case Intents.UtilitiesReject:
                    return await waterfallStep.ReplaceDialogAsync(DialogIds.EnterUserNameDialog, waterfallStep.Values["UserProfile"], cancellationToken);

                default:
                    await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("I don't understand, let's try again"));
                    return await waterfallStep.ReplaceDialogAsync(DialogIds.EnterUserNameDialog, waterfallStep.Values["UserProfile"], cancellationToken);
            }
        }
    }
}
