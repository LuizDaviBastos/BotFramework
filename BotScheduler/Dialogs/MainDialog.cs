using BotScheduler.API;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
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
        private readonly IConfiguration _configuration;
        private readonly MemoryStorage _memoryStorage;

        public MainDialog(UserState userState,
            MemoryStorage memoryStorage, IConfiguration configuration) : base(nameof(MainDialog))
        {
            this._configuration = configuration;
            _memoryStorage = memoryStorage;
            _statePropertyAccessor = userState.CreateProperty<Appointment>(nameof(Appointment));

            var waterfallSteps = new WaterfallStep[]
            {
                //dialogs...
                QuestionSchedulesAsync,
                IntentAsync,
                Finish,
                Finish2

            };

            AddDialog(new WaterfallDialog(DialogIds.MainWaterfallDialog, waterfallSteps));
            //AddDialog(new CreateAppointmentDialog(DialogIds.CreateAppointmentDialog));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));

            InitialDialogId = DialogIds.MainWaterfallDialog;
        }


        public async Task<DialogTurnResult> QuestionSchedulesAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"What's your name?")
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> IntentAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["Name"] = waterfallStep.Result;
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Now tour age:")
            }, cancellationToken);
        }


        public async Task<DialogTurnResult> Finish(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["Age"] = waterfallStep.Result;
            var reponse = waterfallStep.Context.Activity.Text;

            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Your name is {waterfallStep.Values["Name"]} and your Age is {waterfallStep.Result}.\n Run Again?")
            }, cancellationToken);
        }


        public async Task<DialogTurnResult> Finish2(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var result = waterfallStep.Result;
            var reponse = waterfallStep.Context.Activity.Text;


            if (reponse == "sim")
            {
                return await waterfallStep.BeginDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
            }
            await waterfallStep.CancelAllDialogsAsync(cancellationToken);
            return await waterfallStep.ReplaceDialogAsync(DialogIds.MainWaterfallDialog, cancellationToken: cancellationToken);
        }
    }
}