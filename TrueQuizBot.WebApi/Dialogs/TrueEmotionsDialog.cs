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
        public TrueEmotionsDialog(IDataProvider dataProvider, IQuestionsProvider questionsProvider, string id) : base(nameof(TrueEmotionsDialog))
        {
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new QuizDialog(questionsProvider, dataProvider));
            AddDialog(new TrueLuckyDialog(nameof(TrueEmotionsDialog)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowTrueEmotionsCard,
                DetermineNextDialog
            }));
            
            InitialDialogId = nameof(WaterfallDialog);
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
                    new Choice("True_задачи"),
                    new Choice("True_везение")
                    
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
                    case "True_везение":
                        return await innerDc.BeginDialogAsync(nameof(TrueLuckyDialog), null, cancellationToken);
                    case "True_задачи":
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}