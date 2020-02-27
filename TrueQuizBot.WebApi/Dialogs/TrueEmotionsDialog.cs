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
                    case Constants.TrueLuckyTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueLuckyDialog));
                    case Constants.TrueTasksTitle:
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog));
                }
            }

            return null;
        }
        
        
        private async Task<DialogTurnResult> ShowTrueEmotionsCard(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var initialText = @"
Приходи на стенд True Engineering за настоящими эмоциями. Попробуй все, поучаствуй везде, собери на каждой станции по стикеру в буклет и получи призы!
За прохождение всех станций — True_футболка.
10 самым быстрым — True_толстовка.

**True_МК (зал «Демо-стейдж»  в 11:15):** расскажем, как с применением Kafka streams и KSQL создать онлайн аналитику логов приложения.

**True_кофе:** ароматный, настоящий, приходи взбодриться.

**True_bot_квиз:** задачки для настоящих инженеров с подарками участникам и победителям.

**True_конструктор:** научим собирать из гигантских кубиков **Куборо** настоящий механизм

**True_викторина:** проверь свой кругозор на больших кнопках

**True_разговоры про технологии** на нашем стенде

Увеличим еще твои шансы на призы?
                                ";
            var promptMessage = MessageFactory.Text(initialText, null, InputHints.ExpectingInput);
            var promptOptions = new PromptOptions
            {
                Prompt = promptMessage,
                Choices = new List<Choice>()
                {
                    new Choice(Constants.TrueTasksTitle),
                    new Choice(Constants.TrueLuckyTitle)
                    
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
                        return await innerDc.BeginDialogAsync(nameof(TrueLuckyDialog), null, cancellationToken);
                    case Constants.TrueTasksTitle:
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}