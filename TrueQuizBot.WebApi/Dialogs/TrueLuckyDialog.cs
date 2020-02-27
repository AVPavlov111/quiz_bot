using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class TrueLuckyDialog : CancelAndHelpDialog
    {
        public TrueLuckyDialog(string id) : base(nameof(TrueLuckyDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowRegistrationMessage,
                ShowFinalMessage,
                DetermineNextDialog
            }));
            
            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> ShowRegistrationMessage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var initialText = @"
Заполни эту анкету и приходи в 16:10 на стенд True Engineering на третьем этаже попытать удачу. Случайным образом мы определим трех везучих обладателей рюкзаков для ноутбука! Твои контакты мы используем для дела, будем звать тебя на наши True_мероприятия.

Кстати, сегодня в 11:15 наши инженеры в зале «Демо-стейдж» рассказывают, как настроили онлайн аналитику логов с применением Kafka streams фреймворка. Приходи послушать!  После МК сможешь потестить инструмент на нашем стенде в любое время.
                                ";
            var promptMessage = MessageFactory.Text(initialText, null, InputHints.ExpectingInput);
            var promptOptions = new PromptOptions
            {
                Prompt = promptMessage
            };

            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        
        private async Task<DialogTurnResult?> ShowFinalMessage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var initialText = @"
Спасибо. Лотерейный билет создан. 

Но, как говорится, на удачу надейся, а сам не плошай. 

Увеличим твои шансы на выигрыш?

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
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                    case Constants.TrueTasksTitle:
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}