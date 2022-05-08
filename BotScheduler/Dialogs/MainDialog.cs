using BotScheduler.API;
using BotScheduler.Dialogs.AppointmentDialog.GetAppointment;
using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Extensions.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        private readonly IStatePropertyAccessor<Appointment> _statePropertyAccessor;
        private readonly LuisRecognizerQuery _luisRecognizerQuery;
        private readonly IConfiguration _configuration;
        private readonly MemoryStorage _memoryStorage;
        private readonly TelehealthApiService _telehealthApiService;
        private readonly RepositoryService repositoryService;

        public MainDialog(UserState userState, LuisRecognizerQuery luisRecognizerQuery,
            MemoryStorage memoryStorage, IConfiguration configuration, TelehealthApiService telehealthApiService, RepositoryService repositoryService) : base(nameof(MainDialog))
        {
            this.repositoryService = repositoryService;
            this._luisRecognizerQuery = luisRecognizerQuery;
            this._configuration = configuration;
            _memoryStorage = memoryStorage;
            _statePropertyAccessor = userState.CreateProperty<Appointment>(nameof(Appointment));
            this._telehealthApiService = telehealthApiService;

            var waterfallSteps = new WaterfallStep[]
            {
                //dialogs...
                QuestionSchedulesAsync,
                IntentAsync,
                Finish

            };

            

            AddDialog(new WaterfallDialog(DialogIds.MainWaterfallDialog, waterfallSteps));
            AddDialog(new CreateAppointmentDialog(DialogIds.CreateAppointmentDialog, this._luisRecognizerQuery, this._telehealthApiService, repositoryService));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            AddDialog(new GetAppointmentDialog(DialogIds.GetAppointmentDialog, this._luisRecognizerQuery, this._configuration, this._telehealthApiService, repositoryService));

            InitialDialogId = DialogIds.MainWaterfallDialog;

            
        }


        public async Task<DialogTurnResult> QuestionSchedulesAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {


            return await CheckIntent(withLuis: false, waterfallStep, cancellationToken);
        }

        public async Task<DialogTurnResult> IntentAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await CheckIntent(withLuis: true, waterfallStep, cancellationToken);
        }

        private async Task<DialogTurnResult> CheckIntent(bool withLuis, WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var response = waterfallStep.Context.Activity.Text;

            switch (response)
            {
                case Intents.CreateAppointment:
                    return await DialogGo.ToCreateAppointment(waterfallStep, cancellationToken);

                case Intents.GetAppointment:
                    return await DialogGo.ToGetAppointment(waterfallStep, cancellationToken);

                default:
                    if (withLuis) return await CheckIntentLuis(waterfallStep, cancellationToken);
                    else
                    {
                        if (waterfallStep.Context.Activity?.Value == DialogIds.Initial)
                            return await waterfallStep.NextAsync(response, cancellationToken);

                        return await waterfallStep.PromptAsync(nameof(TextPrompt), new PromptOptions() { Prompt = MessageFactory.Text("How may I help you?") }, cancellationToken);
                    }
            }
        }

        private async Task<DialogTurnResult> CheckIntentLuis(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            if (!_luisRecognizerQuery.IsConfigured)
            {
                await waterfallStep.Context.SendActivityAsync("LUIS not configured, please try again");
                await waterfallStep.ReplaceDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
            }

            //Luis, help with this consultation here
            var intent = await _luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);

            switch (intent.GetTopScoringIntent().intent)
            {
                case Intents.CreateCalendarEntry:
                    return await DialogGo.ToCreateAppointment(waterfallStep, cancellationToken);

                case Intents.FindCalendarEntry:
                    return await DialogGo.ToGetAppointment(waterfallStep, cancellationToken);

                case "None":
                    return await DialogGo.ToDontUnderstand(waterfallStep, cancellationToken);

                default:
                    return await DialogGo.ToDontUnderstand(waterfallStep, cancellationToken);
            }
        }


        public async Task<DialogTurnResult> Finish(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var resultLuis = await _luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);
            var intent = resultLuis.GetTopScoringIntent().intent;

            if (intent == Intents.Calendar_Confirm || intent == Intents.UtilitiesConfirm)
                return await waterfallStep.BeginDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);

            else
            {
                await waterfallStep.CancelAllDialogsAsync(cancellationToken);
                return await waterfallStep.ReplaceDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
            }

        }
    }
}