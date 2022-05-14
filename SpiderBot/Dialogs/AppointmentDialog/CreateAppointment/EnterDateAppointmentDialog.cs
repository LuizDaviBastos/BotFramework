using BotScheduler.API;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs.AppointmentDialog.CreateAppointment
{
    public class EnterDateAppointmentDialog : ComponentDialog
    {
        private LuisRecognizerQuery _luisRecognizerQuery;
        private readonly TelehealthApiService _telehealthApiService;
        private readonly RepositoryService repositoryService;

        public EnterDateAppointmentDialog(string dialogId, LuisRecognizerQuery luisRecognizerQuery,
            TelehealthApiService telehealthApiService, RepositoryService repositoryService) : base(dialogId)
        {
            this._luisRecognizerQuery = luisRecognizerQuery;
            this._telehealthApiService = telehealthApiService;
            this.repositoryService = repositoryService;
            var waterfallDialog = new WaterfallStep[]
            {
                GetDateAppointmentAsync,
                QuestionDateAppointmentAsync,
                LoopDateAppointmentAsync
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallDialog));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new TextPrompt(nameof(TextPrompt)));
        }

        public async Task<DialogTurnResult> GetDateAppointmentAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["UserProfile"] = waterfallStep.Options;

            var times = await this._telehealthApiService.GetTimes();
            var choiceList = new List<Choice>();
            times.ForEach(s => { choiceList.Add(new Choice(s)); });

            var message = $"Choose appointment time";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(message),
                Choices = choiceList,
                Style = ListStyle.Auto
            });
        }

        public async Task<DialogTurnResult> QuestionDateAppointmentAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            dynamic choice = waterfallStep.Result;
            var date = choice.Value;

            (waterfallStep.Values["UserProfile"] as UserProfile).DateAppointment = date;

            var message = $"Confirm date \"{date}\"?";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> LoopDateAppointmentAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var intents = await this._luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);

            switch (intents.GetTopScoringIntent().intent)
            {
                case Intents.UtilitiesConfirm:
                    {
                        var message = $"Confirmed";
                        await repositoryService.MessageAddAsync(message, "Patient", waterfallStep.Context);
                        return await waterfallStep.EndDialogAsync(waterfallStep.Values["UserProfile"], cancellationToken);
                    }
                default:
                    return await waterfallStep.ReplaceDialogAsync(DialogIds.EnterDateAppointmentDialog, waterfallStep.Values["UserProfile"], cancellationToken);
            }
        }
    }
}
