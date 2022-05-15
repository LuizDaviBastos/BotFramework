using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library.Keys;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;
using SpiderBot.Data.Interfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{
    public class NewAnnouncementDialog : ComponentDialog
    {
        private readonly IAnnouncementRepository announcementRepository;
        private readonly ITitleRepository titleRepository;

        public NewAnnouncementDialog(IAnnouncementRepository announcementRepository, ITitleRepository titleRepository) : base(nameof(NewAnnouncementDialog))
        {
            this.announcementRepository = announcementRepository;
            this.titleRepository = titleRepository;

            var watterfallDialogs = new WaterfallStep[]
            {
                EnterService,
                EnterPrice,
                EnterAccess
            };

            AddDialog(new WaterfallDialog(DialogIds.WaterfallDialog, watterfallDialogs));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            AddDialog(new NumberPrompt<long>(DialogIds.NumberPrompt, PriceValidatorAsync));
            AddDialog(new ChoicePrompt(DialogIds.ChoicePrompt));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            InitialDialogId = DialogIds.WaterfallDialog;
        }

        public async Task<DialogTurnResult> EnterService(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var listTitle = (await this.titleRepository.Get()).Select(x => x.Name).ToList();
            var listChoice = new List<Choice>();

            listTitle.ForEach(name => listChoice.Add(new Choice(name)));

            return await waterfallStep.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Qual combo está compartilhando?"),
                Choices = listChoice,
                Style = ListStyle.SuggestedAction

            }, cancellationToken);
        }

        private static Task<bool> PriceValidatorAsync(PromptValidatorContext<long> promptContext, CancellationToken cancellationToken)
        {
            var value = promptContext.Recognized.Value;
            return Task.FromResult(promptContext.Recognized.Succeeded && value > 0 && (value.ToString()).Length > 1);
        }

        public async Task<DialogTurnResult> EnterPrice(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.PromptAsync(DialogIds.NumberPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Qual Valor?")
            }, cancellationToken);
        }

        public async Task<DialogTurnResult> EnterAccess(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            /*  var listChoice = new List<Choice>
              {
                  new Choice("Login e senha"),
                  new Choice("Convite por email"),
                  new Choice("Ativação por código")
              };

              return await waterfallStep.PromptAsync(nameof(ChoicePrompt), new PromptOptions()
              {
                  Prompt = MessageFactory.Text($"Qual forma de acesso?"),
                  Choices = listChoice,
                  Style = ListStyle.HeroCard

              }, cancellationToken);*/

            await DialogGo.ToSendWelcomeActivity(waterfallStep.Context, cancellationToken);
            return await DialogGo.ToMainDialog(waterfallStep, cancellationToken);
        }



    }
}
