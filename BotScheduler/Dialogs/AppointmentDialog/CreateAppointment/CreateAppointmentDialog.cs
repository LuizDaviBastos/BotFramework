using BotScheduler.Dialogs.CreateAppointment;
using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library;
using BotScheduler.Library.Keys;
using BotScheduler.Models;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{
    public class CreateAppointmentDialog : ComponentDialog
    {
        private readonly RepositoryService repositoryService;

        public CreateAppointmentDialog(string dialogId) : base(dialogId)
        {
            var watterfallDialogs = new WaterfallStep[]
            {
                QuestionIntentionForVerificationOfBenefitsAsync,
              /*  CollectInsuranceInfo,
                GetDateAppointmentAsync,
                GetUserNameAsync,
                GetEmailAsync,
                GetPhoneNumberAsync,
                EndDialog*/
            };

            AddDialog(new WaterfallDialog(DialogIds.WaterfallDialog, watterfallDialogs));
            //AddDialog(new EnterUserNameDialog(DialogIds.EnterUserNameDialog, this._luisRecognizerQuery, repositoryService));
           // AddDialog(new TextPrompt($"{DialogIds.TextPrompt}PhoneNumber", PhoneNumberValidatorAsync));
            AddDialog(new TextPrompt(DialogIds.TextPrompt));
            AddDialog(new ChoicePrompt(DialogIds.ChoicePrompt));
            InitialDialogId = DialogIds.WaterfallDialog;
        }

        public async Task<DialogTurnResult> QuestionIntentionForVerificationOfBenefitsAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Do you want to verify the benefits?")
            }, cancellationToken);
        }

       /* public async Task<DialogTurnResult> CollectInsuranceInfo(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var intents = await this._luisRecognizerQuery.RecognizeAsync(waterfallStep.Context, cancellationToken);

            if (intents.GetTopScoringIntent().intent == Intents.UtilitiesConfirm)
                return await waterfallStep.BeginDialogAsync(DialogIds.InsuranceDialog, cancellationToken: cancellationToken);

            return await waterfallStep.ContinueDialogAsync(cancellationToken);
        }

        public async Task<DialogTurnResult> GetDateAppointmentAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            return await waterfallStep.BeginDialogAsync(DialogIds.EnterDateAppointmentDialog, new UserProfile(), cancellationToken);
        }

        public async Task<DialogTurnResult> GetUserNameAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var userProfile = (UserProfile)waterfallStep.Result;
            return await waterfallStep.BeginDialogAsync(DialogIds.EnterUserNameDialog, userProfile, cancellationToken);
        }

        public async Task<DialogTurnResult> GetEmailAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            waterfallStep.Values["UserProfile"] = waterfallStep.Result;

            return await waterfallStep.PromptAsync(DialogIds.TextPrompt, new PromptOptions()
            {
                Prompt = MessageFactory.Text("Now enter your e-mail address"),
                RetryPrompt = MessageFactory.Text("E-mail address incorrect, try again")
            });
        }

        private static Task<bool> PhoneNumberValidatorAsync(PromptValidatorContext<string> promptContext, CancellationToken cancellationToken)
        {
            return Task.FromResult(promptContext.Recognized.Succeeded);
        }

        public async Task<DialogTurnResult> GetPhoneNumberAsync(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {
            var user = (waterfallStep.Values["UserProfile"] as UserProfile);
            (waterfallStep.Values["UserProfile"] as UserProfile).Email = (string)waterfallStep.Result;

            return await waterfallStep.PromptAsync($"{ DialogIds.TextPrompt}PhoneNumber", new PromptOptions()
            {
                Prompt = MessageFactory.Text($"Ok {user.Name}, now enter your phone number"),
                RetryPrompt = MessageFactory.Text("Incorrect entry, please try again")
            });
        }


        public async Task<DialogTurnResult> EndDialog(WaterfallStepContext waterfallStep, CancellationToken cancellationToken)
        {

            (waterfallStep.Values["UserProfile"] as UserProfile).Phone = waterfallStep.Result?.ToString().Replace("-", "").Replace(" ", "");

            //this is all info from user
            var userInfo = (UserProfile)waterfallStep.Values["UserProfile"];

            await QueueManager.AddMessageAsync(userInfo);

            var result = await this._telehealthApiService.PostAppointmentAsync(new BaseAppointment()
            {
                Patients = new List<Patient>() { new Patient() { FullName = userInfo.Name, Email = userInfo.Email, PhoneNumber = userInfo.Phone } },
                StartDateTime = userInfo.DateAppointment.ToDateTime()
            });

            if (result != null && result.Success)
            {
                return await DialogGo.ToMainDialog(waterfallStep, cancellationToken, "Your appointment was successfully created!");
            }

            else
            {
                return await DialogGo.ToMainDialog(waterfallStep, cancellationToken, "There was a problem creating the appointment");
            }
        }*/
    }
}
