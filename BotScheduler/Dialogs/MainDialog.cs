using BotScheduler.API;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
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
                QuestionName,
                QuestionAge,
                QuestionRunAgain,
                RunAgain

            };

            AddDialog(new WaterfallDialog(DialogIds.MainWaterfallDialog, waterfallSteps));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = DialogIds.MainWaterfallDialog;
        }


        public async Task<DialogTurnResult> QuestionName(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"What's your name?")
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> QuestionAge(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["Name"] = waterfallStep.Result;
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Now tour age:")
            }, cancellationToken);
        }


        public async Task<DialogTurnResult> QuestionRunAgain(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["Age"] = waterfallStep.Result;
            var reponse = waterfallStep.Context.Activity.Text;

            var choiceList = new List<Choice>()
            {
                new Choice("Sim"),
                new Choice("Não"),
            };

            return await waterfallStep.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Your name is {waterfallStep.Values["Name"]} and your Age is {waterfallStep.Result}.\n Run Again?"),
                Choices = choiceList,
                Style = ListStyle.Auto
            });
        }


        public async Task<DialogTurnResult> RunAgain(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
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