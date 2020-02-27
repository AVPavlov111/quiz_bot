using System;
using System.Collections.Generic;
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
        private const string TrueImg = "https://truebotwebapp.azurewebsites.net/true.png";
        private const string FalseImg = "https://truebotwebapp.azurewebsites.net/false.png";
        private const string ChoiceText = "Выберите один из вариантов ответа";
        private const string TextAnswer = "";

        public QuizDialog(IQuestionsProvider questionsProvider, IDataProvider dataProvider)
            : base(nameof(QuizDialog))
        {
            _questionsProvider = questionsProvider;
            _dataProvider = dataProvider;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowQuestion,
                CheckAnswer,
                OriginStepAsync
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> ShowQuestion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((await _dataProvider.GetCompletedQuestionsIndexes(GetUserId(stepContext))).Any() == false)
            {
                var activity = Activity.CreateMessageActivity();
                activity.Text = "";
                await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            }
            
            var question = await _questionsProvider.GetQuestion(GetUserId(stepContext));

            if (question == null)
            {
                return await stepContext.EndDialogAsync(null, cancellationToken);
            }

            _question = question;

            await ShowQuestionText(stepContext, cancellationToken);

            if (_question.QuestionType == QuestionType.Choice)
            {
                return await ShowChices(stepContext, cancellationToken);
            }

            return await ShowQuestionWithTextAnswer(stepContext, cancellationToken);
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

            await _dataProvider.SaveAnswer(stepContext.Context.Activity.From.Id, _question, answer);

            await ShowAnswerImage(stepContext, cancellationToken, _question.IsCorrectAnswer(answer));

            var promptMessage = MessageFactory.Text("", "", InputHints.ExpectingInput);
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Some text 3", "Some text 4", InputHints.ExpectingInput);
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
        
        private async Task ShowQuestionText(DialogContext stepContext, CancellationToken cancellationToken)
        {
            var activity = Activity.CreateMessageActivity();
            activity.Text = _question.Text;
            activity.TextFormat = "xml";

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
        }

        private static string GetUserId(DialogContext stepContext)
        {
            return stepContext.Context.Activity.From.Id;
        }

        private async Task<DialogTurnResult> ShowChices(DialogContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text(ChoiceText, ChoiceText, InputHints.AcceptingInput);
            var promptOptions = new PromptOptions
            {
                Prompt = promptMessage,
                Choices = _question.Answers.Select(a => new Choice { Value = a }).ToList()
            };
            return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
        }
        
        private static async Task<DialogTurnResult> ShowQuestionWithTextAnswer(DialogContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text(TextAnswer, TextAnswer, InputHints.AcceptingInput);
            var promptOptions = new PromptOptions { Prompt = promptMessage };
            return await stepContext.PromptAsync(nameof(TextPrompt), promptOptions, cancellationToken);
        }
        
        private static async Task ShowAnswerImage(DialogContext stepContext, CancellationToken cancellationToken, bool isAnswerCorrect)
        {
            var activity = Activity.CreateMessageActivity();
            activity.Attachments = new List<Attachment>
            {
                new Attachment
                {
                    ContentUrl = isAnswerCorrect ? TrueImg : FalseImg,
                    ContentType = "image/png",
                    Name = ""
                }
            };

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
        }
    }
}