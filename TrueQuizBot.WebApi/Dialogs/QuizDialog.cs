using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Activity = Microsoft.Bot.Schema.Activity;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class QuizDialog : CancelAndHelpDialog
    {
        private readonly IQuestionsProvider _questionsProvider;
        private readonly IDataProvider _dataProvider;
        private Question? _question;
        private const string ChoiceText = "Выберите один из вариантов ответа";
        private const string TextAnswer = "";

        public QuizDialog(IQuestionsProvider questionsProvider, IDataProvider dataProvider)
            : base(nameof(QuizDialog))
        {
            _questionsProvider = questionsProvider;
            _dataProvider = dataProvider;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowQuestion,
                CheckAnswer,
                OriginStepAsync
            }));

            // The initial child Dialog to run.
            InitialDialogId = nameof(WaterfallDialog);
        }

        private async Task<DialogTurnResult> ShowQuestion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var question = _questionsProvider.GetQuestion(stepContext.Context.Activity.From.Id);

            if (question == null)
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            _question = question;

            var activity = Activity.CreateMessageActivity();
            activity.Text = _question.Text;

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);


            if (_question.QuestionType == QuestionType.Choice)
            {
                var promptMessage = MessageFactory.Text(ChoiceText, ChoiceText, InputHints.AcceptingInput);
                var retryActivity = new Activity(ActivityTypes.Message) { Text = "retry text" };
                var promptOptions = new PromptOptions
                {
                    Prompt = promptMessage,
                    Choices = _question.Answers.Select(a => new Choice { Value = a }).ToList(),
                    RetryPrompt = retryActivity
                };
                return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }
            else
            {
                var promptMessage = MessageFactory.Text(TextAnswer, TextAnswer, InputHints.AcceptingInput);
                var promptOptions = new PromptOptions { Prompt = promptMessage };
                return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> CheckAnswer(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            string answer;
            if (stepContext.Result is FoundChoice choice)
            {
                answer = choice.Value;
            }
            else
            {
                answer = (string) stepContext.Result;
            }

            if (_question == null)
            {
                throw new Exception("_question is null");
            }

            _dataProvider.SaveAnswer(stepContext.Context.Activity.From.Id, _question, answer);
            var completeQuestionText = "НЕТ";
            if (_question.IsCorrectAnswer(answer))
            {
                completeQuestionText = "ДА!";
            }

            var activity = Activity.CreateMessageActivity();
            activity.Text = completeQuestionText;

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            var promptMessage = MessageFactory.Text(completeQuestionText, completeQuestionText, InputHints.ExpectingInput);
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Some text 3", "Some text 4", InputHints.ExpectingInput);
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
    }
}