using BotScheduler.API;
using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using BotScheduler.Recognizer;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs.AppointmentDialog.GetAppointment
{
    public class GetAppointmentDialog : ComponentDialog
    {
        private readonly LuisRecognizerQuery luisRecognizerQuery;
        private readonly IConfiguration _configuration;
        private readonly TelehealthApiService telehealthApiService;
        private readonly RepositoryService repositoryService;

        public GetAppointmentDialog(string dialogId, LuisRecognizerQuery luisRecognizerQuery, IConfiguration configuration, TelehealthApiService telehealthApiService, RepositoryService repositoryService) : base(dialogId)
        {
            this.repositoryService = repositoryService;
            this.luisRecognizerQuery = luisRecognizerQuery;
            this._configuration = configuration;
            this.telehealthApiService = telehealthApiService;

            var waterfallDialogs = new WaterfallStep[]
            {
                SymptomsAsync,
                SymptomsResultAsync,
                ConnectDeviceAsync,
                GetPhoneNumberAsync,
                GetAppointment
            };

            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), waterfallDialogs));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            AddDialog(new DeviceIntegrationDialog(DialogIds.DeviceIntegrationDialog, this.luisRecognizerQuery, this.telehealthApiService, repositoryService));
            AddDialog(new FeverDialog(DialogIds.FeverDialog, this.luisRecognizerQuery, this.telehealthApiService, repositoryService));
            InitialDialogId = nameof(WaterfallDialog);
        }

        public async Task<DialogTurnResult> SymptomsAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var message = "Write about your symptoms:";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            });
        }

        public async Task<DialogTurnResult> SymptomsResultAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var intents = await this.luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);

            if (intents.GetTopScoringIntent().intent == Intents.Symptoms_Fever)
            {
                return await waterfallStep.BeginDialogAsync(DialogIds.FeverDialog, cancellationToken: cancellationToken);
            }

            return await waterfallStep.ContinueDialogAsync(cancellationToken);
        }

        public async Task<DialogTurnResult> ConnectDeviceAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.BeginDialogAsync(DialogIds.DeviceIntegrationDialog, cancellationToken: cancellationToken);
        }

        public async Task<DialogTurnResult> GetPhoneNumberAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("Lets's verify if you have an appointment"));

            var message = "Please type your phone number";
            await repositoryService.MessageAddAsync(message, "bot", waterfallStep.Context);

            return await waterfallStep.PromptAsync(nameof(TextPrompt), new PromptOptions()
            {
                Prompt = MessageFactory.Text(message)
            });
        }

        public async Task<DialogTurnResult> GetAppointment(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var phoneNumber = (string)waterfallStep.Result;
            await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("Please wait a moment..."), cancellationToken);

            var queryResult = await this.telehealthApiService.FindAppointmentAsync(phoneNumber);
            var result = await Feature.CreateSessionIdAndGroupId(queryResult, this.telehealthApiService);

            await repositoryService.ChatUpdateSessionAsync(queryResult.Item.Id, waterfallStep.Context);

            if (result != null && result.Success)
            {
                var url = Feature.GetUrlChat(result.Item.Id, this._configuration);
                var button = new List<CardAction>()
                {
                    new CardAction()
                    {
                        Title = "Start video chat",
                        Type = ActionTypes.OpenUrl,
                        Value = url
                    }
                };

                await waterfallStep.Context.SendActivityAsync(MessageFactory.Text("Ok, you have an appointment"), cancellationToken);
                await waterfallStep.Context.SendActivityAsync(MessageFactory.Attachment(new HeroCard()
                {
                    Title = result?.Item?.Patients?.First()?.FullName ?? "Undefined",
                    Subtitle = $"From number: {phoneNumber}",
                    Text = $"Date: {result.Item.StartDateTime.ToLongDateString()}",
                    Buttons = button

                }.ToAttachment()));
                return await DialogGo.ToMainDialog(waterfallStep, cancellationToken);
            }
            return await DialogGo.ToMainDialog(waterfallStep, cancellationToken, "No appointment found");
        }
    }
}
