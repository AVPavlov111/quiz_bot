using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class TrueEmotionsDialog : CancelAndHelpDialog
    {
        public TrueEmotionsDialog() 
            : base(nameof(TrueEmotionsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowTrueEmotionsCard,
                DetermineNextDialog
            }));
            
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowTrueEmotionsCard(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {

            var text = @"
Приходи на стенд True Engineering за настоящими эмоциями. Попробуй все, поучаствуй везде, собери на каждой станции по стикеру в буклет и получи призы!

За прохождение всех станций — True_футболка.

10 самым быстрым — True_толстовка.";
            var activity = Activity.CreateMessageActivity();
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = "**True\\_МК (зал «Демо-стейдж»  в 11:15):** расскажем, как с применением Kafka streams и KSQL создать онлайн аналитику логов приложения.";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = "**True\\_кофе:** ароматный, настоящий, приходи взбодриться.";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            text = "**True\\_bot\\_квиз:** задачки для настоящих инженеров с подарками участникам и победителям.";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = "**True\\_конструктор:** научим собирать из гигантских кубиков **Куборо** настоящий механизм";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = "**True\\_викторина:** проверь свой кругозор на больших кнопках";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = "**True\\_разговоры про технологии** на нашем стенде";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = "Увеличим еще твои шансы на призы?";
            var promptMessage = MessageFactory.Text(text, null, InputHints.ExpectingInput);
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
        
        private async Task<DialogTurnResult?> DetermineNextDialog(DialogContext innerDc, CancellationToken cancellationToken)
        {
            if (innerDc.Context.Activity.Type == ActivityTypes.Message)
            {
                var text = innerDc.Context.Activity.Text;

                switch (text)
                {
                    case Constants.TrueLuckyTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueLuckyDialog), new TrueLuckyPersonalData(), cancellationToken);
                    case Constants.TrueTasksTitle:
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}