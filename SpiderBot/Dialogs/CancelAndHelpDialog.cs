using BotScheduler.Dialogs.GoToDialog;
using BotScheduler.Library.Keys;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using System.Threading;
using System.Threading.Tasks;

namespace BotScheduler.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        public CancelAndHelpDialog(string name) : base(name) { }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            //will called every shift
            var resultInterrupt = await InterruptAsync(innerDc, cancellationToken);

            if (resultInterrupt != null) return resultInterrupt;


            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        public async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            //will called every shift
            if (innerDc?.Context?.Activity?.Type == ActivityTypes.Message)
            {
                var text = innerDc?.Context?.Activity?.Text?.ToLowerInvariant();
                switch (text)
                {
                    case Intents.Cancel:
                        return await DialogGo.ToCancelDialog(innerDc, cancellationToken);
                }
        }
            return null;
        }
    }
}
