using BotScheduler.API;
using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Extensions.Configuration;
using SpiderBot.Data.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{
    public class MainDialog : CancelAndHelpDialog
    {
        private readonly IStatePropertyAccessor<Appointment> _statePropertyAccessor;
        private readonly IConfiguration _configuration;
        private readonly IAnnouncementRepository announcementRepository;
        private readonly ITitleRepository titleRepository;
        private readonly MemoryStorage _memoryStorage;

        public MainDialog(UserState userState,
            MemoryStorage memoryStorage, IConfiguration configuration, IAnnouncementRepository announcementRepository, ITitleRepository titleRepository) : base(nameof(MainDialog))
        {
            this._configuration = configuration;
            _memoryStorage = memoryStorage;
            _statePropertyAccessor = userState.CreateProperty<Appointment>(nameof(Appointment));
            this.announcementRepository = announcementRepository;
            this.titleRepository = titleRepository;

            var waterfallSteps = new WaterfallStep[]
            {
                //dialogs...
                InitialQuestion,
                VerifyIntent,
                QuestionRunAgain,
                RunAgain

            };

            AddDialog(new WaterfallDialog(DialogIds.MainWaterfallDialog, waterfallSteps));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            AddDialog(new NewAnnouncementDialog(announcementRepository, titleRepository));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));

            InitialDialogId = DialogIds.MainWaterfallDialog;
        }


        public async Task<DialogTurnResult> InitialQuestion(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var choiceList = new List<Choice>();

            Intents.AnnouncementIntents.GetIntents().ForEach(option => choiceList.Add(new Choice(option)));

            return await waterfallStep.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Escolha uma opção"),
                Choices = choiceList,
                Style = ListStyle.SuggestedAction
            });
        }

        public async Task<DialogTurnResult> VerifyIntent(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            switch (waterfallStep.Context.Activity.Text)
            {
                case Intents.AnnouncementIntents.NewAnnouncement:
                    return await waterfallStep.ReplaceDialogAsync(nameof(NewAnnouncementDialog), null, cancellationToken);
                case Intents.AnnouncementIntents.ListAnnouncement:
                    return await DialogGo.ToDontUnderstand(waterfallStep, cancellationToken);
                case Intents.AnnouncementIntents.EditAnnouncement:
                    return await DialogGo.ToDontUnderstand(waterfallStep, cancellationToken);
                case Intents.AnnouncementIntents.DeleteAnnouncement:
                    return await DialogGo.ToDontUnderstand(waterfallStep, cancellationToken);
                default:
                    return await DialogGo.ToDontUnderstand(waterfallStep, cancellationToken);
            }
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