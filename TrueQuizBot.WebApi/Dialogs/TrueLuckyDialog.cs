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
        private readonly IDataProvider _dataProvider;
        private const string DisplayNameText = "Твои фамилия и имя";
        private const string PhoneNumberText = "Телефон";
        private const string CompanyNameText = "В какой компании работаешь?";
        private const string PositionText = "На какой должности?";
        private const string InterestsText = "Какой стек технологий тебе интересен?";
        private const string IsAcceptedText = "Согласны на обработку персональных данных? https://truebotwebapp.azurewebsites.net/personalDataAcceptance.htm";

        public TrueLuckyDialog(IDataProvider dataProvider) : base(nameof(TrueLuckyDialog))
        {
            _dataProvider = dataProvider;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                ShowGreetingMessage,
                PhoneNumberStep,
                CompanyNameStep,
                PositionStep,
                InterestsStep,
                AcceptanceStep,
                ShowFinalMessage,
                DetermineNextDialog
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> ShowGreetingMessage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var initialText = @"
Заполни эту анкету и приходи в 16:10 на стенд True Engineering на третьем этаже попытать удачу. Случайным образом мы определим трех везучих обладателей рюкзаков для ноутбука! Твои контакты мы используем для дела, будем звать тебя на наши True_мероприятия.

:stuck_out_tongue_winking_eye: Кстати, сегодня в 11:15 наши инженеры в зале «Демо-стейдж» рассказывают, как настроили онлайн аналитику логов с применением Kafka streams фреймворка. Приходи послушать!  После МК сможешь потестить инструмент на нашем стенде в любое время.
";
            
            var personalData = (TrueLuckyPersonalData)stepContext.Options;

            var activity = Activity.CreateMessageActivity();
            activity.Text = initialText;
            await stepContext.Context.SendActivityAsync(activity, cancellationToken);

            if (personalData.DisplayName == null)
            {
                var promptMessage = MessageFactory.Text(DisplayNameText, DisplayNameText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.DisplayName, cancellationToken);
        }

        private async Task<DialogTurnResult> PhoneNumberStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (TrueLuckyPersonalData) stepContext.Options;

            personalData.DisplayName = (string) stepContext.Result;

            if (personalData.PhoneNumber == null)
            {
                var promptMessage = MessageFactory.Text(PhoneNumberText, PhoneNumberText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = promptMessage}, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.PhoneNumber, cancellationToken);
        }

        private async Task<DialogTurnResult> CompanyNameStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (TrueLuckyPersonalData) stepContext.Options;

            personalData.PhoneNumber = (string) stepContext.Result;

            if (personalData.CompanyName == null)
            {
                var promptMessage = MessageFactory.Text(CompanyNameText, CompanyNameText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = promptMessage}, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.CompanyName, cancellationToken);
        }

        private async Task<DialogTurnResult> PositionStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (TrueLuckyPersonalData) stepContext.Options;

            personalData.CompanyName = (string) stepContext.Result;

            if (personalData.Position == null)
            {
                var promptMessage = MessageFactory.Text(PositionText, PositionText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = promptMessage}, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.Position, cancellationToken);
        }

        private async Task<DialogTurnResult> InterestsStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (TrueLuckyPersonalData) stepContext.Options;

            personalData.Position = (string) stepContext.Result;

            if (personalData.Interests == null)
            {
                var promptMessage = MessageFactory.Text(InterestsText, InterestsText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions {Prompt = promptMessage}, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.Interests, cancellationToken);
        }

        private async Task<DialogTurnResult> AcceptanceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (TrueLuckyPersonalData) stepContext.Options;

            personalData.Interests = (string) stepContext.Result;

            var promptMessage = MessageFactory.Text(IsAcceptedText, IsAcceptedText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions {Prompt = promptMessage}, cancellationToken);
        }

        private async Task<DialogTurnResult?> ShowFinalMessage(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool) stepContext.Result)
            {
                var personalData = (TrueLuckyPersonalData) stepContext.Options;
                await _dataProvider.SavePersonalDataFromTrueLucky(stepContext.Context.Activity.From.Id, personalData);

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
                        new Choice(Constants.TrueEmotionsTitle)
                    }
                };

                return await stepContext.PromptAsync(nameof(ChoicePrompt), promptOptions, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
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
                    case Constants.TrueTasksTitle:
                        return await innerDc.BeginDialogAsync(nameof(QuizDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}