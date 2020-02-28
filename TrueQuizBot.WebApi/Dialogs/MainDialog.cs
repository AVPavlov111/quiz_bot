using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        public MainDialog(IDataProvider dataProvider, IQuestionsProvider questionsProvider)
            : base(nameof(MainDialog))
        {
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new QuizDialog(questionsProvider, dataProvider));
            AddDialog(new TrueEmotionsDialog());
            AddDialog(new TrueLuckyDialog(dataProvider));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AcceptanceStep,
                DetermineNextDialog
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> AcceptanceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            const string initialText = "Привет! Я True_Bot, знаю задачки на логику, могу проверить уровень твоего везения, а еще – рассказать про True_инженерные_эмоции на SnowOne. В каждом варианте мы припасли для тебя приятные сувениры и полезные подарки. Во что больше веришь: в инженерное мышление, удачу или активность? Чем займемся?";
            var promptMessage = MessageFactory.Text(initialText, null, InputHints.ExpectingInput);
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
                    case Constants.TrueEmotionsTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueEmotionsDialog), null, cancellationToken);
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