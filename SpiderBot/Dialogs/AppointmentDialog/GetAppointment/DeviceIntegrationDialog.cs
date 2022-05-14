using BotScheduler.API;
using BotScheduler.Library.Keys;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace BotScheduler.Dialogs
{
    public class DeviceIntegrationDialog : ComponentDialog
    {
        private readonly LuisRecognizerQuery _luisRecognizerQuery;
        private readonly TelehealthApiService _telehealthApiService;
        private readonly RepositoryService repositoryService;

        public DeviceIntegrationDialog(string dialogId, LuisRecognizerQuery luisRecognizerQuery, TelehealthApiService telehealthApiService, RepositoryService repositoryService) : base(dialogId)
        {
            this._luisRecognizerQuery = luisRecognizerQuery;
            this._telehealthApiService = telehealthApiService;
            this.repositoryService = repositoryService;
            var watterfallDialogs = new WaterfallStep[]
            {
                ConnectYourDeviceAsync,
                EndDialogAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), watterfallDialogs));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            InitialDialogId = nameof(WaterfallDialog);
        }


        public async Task<DialogTurnResult> ConnectYourDeviceAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = $"Do you want to connect your device?";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            { Prompt = MessageFactory.Text(message) }, cancellationToken);
        }

        public async Task<DialogTurnResult> EndDialogAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var intents = await this._luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);

            if (intents.GetTopScoringIntent().intent == Intents.UtilitiesConfirm)
            {
                var message = $"Apple Watch is connected. \n\n{DateTime.Now.ToString("dd/MM/yy hh:mm")}. \n\nHR - 87 bpm \n\nO2 - 98%";
                await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

                await waterfallStep.Context.SendActivityAsync(MessageFactory.Text(message));
            }

            return await waterfallStep.EndDialogAsync();
        }
    }
}
