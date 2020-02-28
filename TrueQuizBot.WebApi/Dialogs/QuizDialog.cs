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
        private const string TrueImg = "https://truebotwebapp.azurewebsites.net/true.png";
        private const string FalseImg = "https://truebotwebapp.azurewebsites.net/false.png";
        private const string ChoiceText = "Выберите один из вариантов ответа";
        private const string TextAnswer = "";
        private const string SkipCommand = "/skip_question";
        private const string IntroMessageId = "IntroMessageId";
        private const string QuizDialogId = "QuizDialogId";
        
        public QuizDialog(IQuestionsProvider questionsProvider, IDataProvider dataProvider)
            : base(nameof(QuizDialog))
        {
            _questionsProvider = questionsProvider;
            _dataProvider = dataProvider;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new RegistrationDialog(dataProvider));
            AddDialog(new WaterfallDialog(IntroMessageId, new WaterfallStep[]
            {
                ShowIntroMessageIfNeed,
                CheckAnswer,
                OriginStepAsync
            }));
            AddDialog(new WaterfallDialog(QuizDialogId, new WaterfallStep[]
            {
                ShowQuestion,
                CheckAnswer,
                OriginStepAsync
            }));

            InitialDialogId = IntroMessageId;
        }
        
        private async Task<DialogTurnResult> ShowIntroMessageIfNeed(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("", "", InputHints.ExpectingInput);
            if ((await _dataProvider.GetCompletedQuestionsIndexes(GetUserId(stepContext))).Any())
            {
                return await stepContext.ReplaceDialogAsync(QuizDialogId, promptMessage, cancellationToken);
            }
            
            var activity = Activity.CreateMessageActivity();
          
            var text = $":nerd_face: Если не знаешь ответ – пропускай вопрос ({SkipCommand}), потом ты сможешь к нему вернуться или оставить без ответа.";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = ":blush: Всем участникам квиза мы приготовили призы. И супер-призы для ТОП-10 в рейтинге. Подробности расскажу позже!";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
            
            text = ":stuck_out_tongue_winking_eye: Кстати, сегодня в 11-15 наши инженеры в зале «Демо-стейдж» рассказывают, как настроили онлайн аналитику с применением Kafka streams фреймворка. Приходи послушать! После МК сможешь потестить инструмент на нашем стенде в любое время.";
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            return await stepContext.ReplaceDialogAsync(QuizDialogId, promptMessage, cancellationToken);
        }
        
        private async Task<DialogTurnResult> ShowQuestion(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var question = await _questionsProvider.GetQuestion(GetUserId(stepContext));

            if (question == null)
            {
                return await stepContext.BeginDialogAsync(nameof(RegistrationDialog), new PersonalData(), cancellationToken);
            }

            await _dataProvider.SaveQurrentQuestionIndex(GetUserId(stepContext), question.Index);

            await ShowQuestionText(stepContext, cancellationToken, question);

            if (question.QuestionType == QuestionType.Choice)
            {
                return await ShowChoices(stepContext, cancellationToken, question);
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

            if (string.Equals(answer, SkipCommand, StringComparison.OrdinalIgnoreCase) == false)
            {
                var question = await _questionsProvider.GetCurrentQuestion(GetUserId(stepContext));
                await _dataProvider.SaveAnswer(stepContext.Context.Activity.From.Id, question, answer);

                await ShowAnswerImage(stepContext, cancellationToken, question.IsCorrectAnswer(answer));
            }

            var promptMessage = MessageFactory.Text("", "", InputHints.ExpectingInput);
            return await stepContext.ReplaceDialogAsync(QuizDialogId, promptMessage, cancellationToken);
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var promptMessage = MessageFactory.Text("Some text 3", "Some text 4", InputHints.ExpectingInput);
            return await stepContext.ReplaceDialogAsync(InitialDialogId, promptMessage, cancellationToken);
        }
        
        private static async Task ShowQuestionText(DialogContext stepContext, CancellationToken cancellationToken, Question? question)
        {
            var activity = Activity.CreateMessageActivity();
            activity.Text = $"{question.Text} \nБаллов: {question.PointsNumber}";
            activity.TextFormat = "xml";

            await stepContext.Context.SendActivityAsync(activity, cancellationToken);
        }

        private static string GetUserId(DialogContext stepContext)
        {
            return stepContext.Context.Activity.From.Id;
        }

        private static async Task<DialogTurnResult> ShowChoices(DialogContext stepContext, CancellationToken cancellationToken, Question? question)
        {
            var promptMessage = MessageFactory.Text(ChoiceText, ChoiceText, InputHints.AcceptingInput);
            var promptOptions = new PromptOptions
            {
                Prompt = promptMessage,
                Choices = question.Answers.Select(a => new Choice { Value = a }).ToList()
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