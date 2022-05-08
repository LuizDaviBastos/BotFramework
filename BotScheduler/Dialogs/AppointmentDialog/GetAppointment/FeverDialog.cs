using BotScheduler.API;
using BotScheduler.Library.Keys;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{
    public class FeverDialog : ComponentDialog
    {
        private readonly LuisRecognizerQuery _luisRecognizerQuery;
        private readonly TelehealthApiService _telehealthApiService;
        private readonly RepositoryService repositoryService;

        public FeverDialog(string dialogId, LuisRecognizerQuery luisRecognizerQuery, TelehealthApiService telehealthApiService, RepositoryService repositoryService) : base(dialogId)
        {
            this._luisRecognizerQuery = luisRecognizerQuery;
            this._telehealthApiService = telehealthApiService;
            this.repositoryService = repositoryService;
            var watterfallDialogs = new WaterfallStep[]
            {
                FeverAsync,
                EndDialogAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), watterfallDialogs));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            InitialDialogId = nameof(WaterfallDialog);
        }


        public async Task<DialogTurnResult> FeverAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = $"How high was the fever ? ";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.EndDialogAsync();
        }
    }
}
