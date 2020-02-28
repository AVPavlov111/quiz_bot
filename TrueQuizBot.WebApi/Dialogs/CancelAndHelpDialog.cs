using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class CancelAndHelpDialog : ComponentDialog
    {
        private const string HelpMsgText = "Show help here";
        private const string CancelMsgText = "Cancelling...";

        public CancelAndHelpDialog(string id)
            : base(id)
        {
        }

        protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
        {
            var result = await InterruptAsync(innerDc, cancellationToken);
            if (result != null)
            {
                return result;
            }

            return await base.OnContinueDialogAsync(innerDc, cancellationToken);
        }

        private async Task<DialogTurnResult?> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text;

                switch (text)
                {
                    
                    case "help":
                    case "Help":
                    case "?":
                        var helpMessage = MessageFactory.Text(HelpMsgText, HelpMsgText, InputHints.ExpectingInput);
                        await innerDc.Context.SendActivityAsync(helpMessage, cancellationToken);
                        return new DialogTurnResult(DialogTurnStatus.Waiting);

                    case "cancel":
                    case "Cancel":
                    case "quit":
                    case "Quit":
                        var cancelMessage = MessageFactory.Text(CancelMsgText, CancelMsgText, InputHints.IgnoringInput);
                        await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);
                        return await innerDc.CancelAllDialogsAsync(cancellationToken);
                    case Constants.TrueLuckyTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueLuckyDialog), new TrueLuckyPersonalData(), cancellationToken);
                    case Constants.TrueTasksTitle:
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                    case Constants.TrueEmotionsTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueEmotionsDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}