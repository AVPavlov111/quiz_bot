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
        private readonly IDataProvider _dataProvider;
        
        private const string HelpMsgText = "Show help here";
        private const string CancelMsgText = "Cancelling...";
        
        public MainDialog(IDataProvider dataProvider, IQuestionsProvider questionsProvider)
            : base(nameof(MainDialog))
        {
            _dataProvider = dataProvider;
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new QuizDialog(questionsProvider, dataProvider));
            AddDialog(new TrueEmotionsDialog(dataProvider, questionsProvider, nameof(TrueEmotionsDialog)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                AcceptanceStep,
                DetermineNextDialog
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> AcceptanceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var initialText = "Привет! Я True_Bot, знаю задачки на логику, могу проверить уровень твоего везения, а еще – рассказать про True_инженерные_эмоции на SnowOne. В каждом варианте мы припасли для тебя приятные сувениры и полезные подарки. Во что больше веришь, в инженерное мышление, удачу, или активность? Чем займемся?";
            var promptMessage = MessageFactory.Text(initialText, null, InputHints.ExpectingInput);
            var promptOptions = new PromptOptions
            {
                Prompt = promptMessage,
                Choices = new List<Choice>()
                {
                    new Choice("True_эмоции"),
                    new Choice("True_везение"),
                    new Choice("True_задачи ")
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
                    case "True_эмоции":
                        return await innerDc.BeginDialogAsync(nameof(TrueEmotionsDialog), null, cancellationToken);
                    case "True_везение":
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                    case "True_задачи":
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                }
            }

            return null;
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _dataProvider.ClearAnswerStatistic(stepContext.Context.Activity.From.Id);
            // Restart the main dialog with a different message the second time around
            var promptMessage = "What else can I do for you?";
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}