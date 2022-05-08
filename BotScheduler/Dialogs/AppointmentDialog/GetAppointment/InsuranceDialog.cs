using BotScheduler.API;
using BotScheduler.Library.Keys;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{

    public class InsuranceDialog : ComponentDialog
    {
        private readonly LuisRecognizerQuery _luisRecognizerQuery;
        private readonly TelehealthApiService _telehealthApiService;
        private readonly RepositoryService repositoryService;

        public InsuranceDialog(string dialogId, LuisRecognizerQuery luisRecognizerQuery, TelehealthApiService telehealthApiService, RepositoryService repositoryService) : base(dialogId)
        {
            this._luisRecognizerQuery = luisRecognizerQuery;
            this._telehealthApiService = telehealthApiService;
            this.repositoryService = repositoryService;
            var watterfallDialogs = new WaterfallStep[]
            {
                GetInsuranceCompanyNameAsync,
                GetInsuranceGroupAsync,
                GetRxBinAsync,
                EndDialogAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), watterfallDialogs));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> GetInsuranceCompanyNameAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = "Enter the Insurance Company:";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> GetInsuranceGroupAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = "Enter the Insurance Group:";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> GetRxBinAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = "Enter the RxBin:";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = "Insurance valid.";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);
            await waterfallStep.Context.SendActivityAsync(MessageFactory.Text(message));
            return await waterfallStep.EndDialogAsync();
        }
    }
}
