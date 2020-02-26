using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class MainDialog : ComponentDialog
    {
        private readonly IDataProvider _dataProvider;

        public MainDialog(QuizDialog quizDialog, RegistrationDialog registrationDialog, IDataProvider dataProvider)
            : base(nameof(MainDialog))
        {
            _dataProvider = dataProvider;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(quizDialog);
            AddDialog(registrationDialog);
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                StartQuiz,
                EnterPersonalData,
                FinalStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> StartQuiz(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            await _dataProvider.AddUser(stepContext.Context.Activity.From.Id);
            return await stepContext.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
        }

        private async Task<DialogTurnResult> EnterPersonalData(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if (await _dataProvider.IsUserAlreadyEnterPersonalData(stepContext.Context.Activity.From.Id))
            {
                return await stepContext.NextAsync(null, cancellationToken);
            }

            return await stepContext.BeginDialogAsync(nameof(RegistrationDialog), new PersonalData(), cancellationToken);
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