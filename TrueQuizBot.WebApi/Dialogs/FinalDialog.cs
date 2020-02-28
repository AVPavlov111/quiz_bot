using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class FinalDialog : CancelAndHelpDialog
    {
        public FinalDialog()
            : base(nameof(FinalDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowFinalCard,
                CheckAnswer
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private static async Task<DialogTurnResult> ShowFinalCard(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var activity = Activity.CreateMessageActivity();

            var text = "Приходи на стенд True Engineering на 3 этаже, вручу юморной сувенир за участие и стикер в твой буклет";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            text = "Если по итогам дня войдешь в ТОП-10 участников квиза, подарю супер-приз. Победителей наградим в 16-10 на нашем стенде на третьем этаже.";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            text = "Приходи пообщаться оффлайн.  Угостим кофе, поиграем в True_викторину,  соберем вместе True_конструктор и поговорим про технологии. ";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            var promptMessage = MessageFactory.Text("Увеличим еще твои шансы на призы?", null, InputHints.ExpectingInput);
            var promptOptions = new PromptOptions
            {
                Prompt = promptMessage,
                Choices = new List<Choice>()
                {
                    new Choice(Constants.TrueEmotionsTitle),
                    new Choice(Constants.TrueLuckyTitle),
                    new Choice(Constants.TrueTasksTitle)
                }
            };

            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }
        
        private async Task<DialogTurnResult> CheckAnswer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
           
            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}