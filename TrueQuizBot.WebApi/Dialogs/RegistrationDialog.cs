using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;
using Activity = Microsoft.Bot.Schema.Activity;

namespace TrueQuizBot.WebApi.Dialogs
{
    public class RegistrationDialog : CancelAndHelpDialog
    {
        private readonly IDataProvider _dataProvider;
        private const string FirstNameText = "Пожалуйста, введите имя";
        private const string LastNameText = "Пожалуйста, введите фамилию";
        private const string PhoneNumberText = "Пожалуйста, введите номер телефона";
        private const string EmailText = "Пожалуйста, введите email";
        private const string IsAcceptedText = "Согласны на обработку персональных данных? https://truebotwebapp.azurewebsites.net/personalDataAcceptance.htm";

        public RegistrationDialog(IDataProvider dataProvider) 
            : base(nameof(RegistrationDialog))
        {
            _dataProvider = dataProvider;
            AddDialog(new TextPrompt(nameof(TextPrompt)));
            AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt)));
            AddDialog(new ChoicePrompt(nameof(ChoicePrompt)));
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FirstNameStep,
                LastNameStep,
                PhoneNumberStep,
                EmailStep,
                AcceptanceStep,
                FinalStep, 
                DetermineNextDialog
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> FirstNameStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (PersonalData)stepContext.Options;

            const string text = "Мои вопросы закончились, теперь расскажи о себе, а я следующим сообщением напишу как и где забрать приз за участие в моей игре .";
            var activity = Activity.CreateMessageActivity();
            activity.Text = text;
            await stepContext.Context.SendActivityAsync(activity);

            if (personalData.FirstName == null)
            {
                var promptMessage = MessageFactory.Text(FirstNameText, FirstNameText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.FirstName, cancellationToken);
        }

        private async Task<DialogTurnResult> LastNameStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (PersonalData)stepContext.Options;

            personalData.FirstName = (string)stepContext.Result;

            if (personalData.LastName == null)
            {
                var promptMessage = MessageFactory.Text(LastNameText, LastNameText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.LastName, cancellationToken);
        }

        private async Task<DialogTurnResult> PhoneNumberStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (PersonalData)stepContext.Options;

            personalData.LastName = (string)stepContext.Result;

            if (personalData.PhoneNumber == null)
            {
                var promptMessage = MessageFactory.Text(PhoneNumberText, PhoneNumberText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.PhoneNumber, cancellationToken);
        }

        private async Task<DialogTurnResult> EmailStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (PersonalData)stepContext.Options;

            personalData.PhoneNumber = (string)stepContext.Result;

            if (personalData.Email == null)
            {
                var promptMessage = MessageFactory.Text(EmailText, EmailText, InputHints.ExpectingInput);
                return await stepContext.PromptAsync(nameof(TextPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
            }

            return await stepContext.NextAsync(personalData.Email, cancellationToken);
        }

        private async Task<DialogTurnResult> AcceptanceStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (PersonalData)stepContext.Options;

            personalData.Email = (string)stepContext.Result;

            var promptMessage = MessageFactory.Text(IsAcceptedText, IsAcceptedText, InputHints.ExpectingInput);

            return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = promptMessage }, cancellationToken);
        }
        
        private async Task<DialogTurnResult> FinalStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            if ((bool) stepContext.Result)
            {
                var personalData = (PersonalData) stepContext.Options;
                personalData.IsAcceptedPersonalDataProcessing = true;
                await _dataProvider.SavePersonalData(stepContext.Context.Activity.From.Id, personalData);
                var currentPositionInRating = await _dataProvider.GetCurrentPosition(stepContext.Context.Activity.From.Id);
                
                var activity = Activity.CreateMessageActivity();

                var text = $@"
Приятно познакомиться виртуально!

Твои контакты мы используем для дела, будем звать тебя на наши True_мероприятия

Спасибо за интересную игру, {personalData.FirstName}! Твое место в рейтинге на данный момент — {currentPositionInRating}
";
                activity.Text = text;
                await stepContext.Context.SendActivityAsync(activity, cancellationToken);                
                
                text = "Приходи на стенд True Engineering на 3 этаже, вручу юморной сувенир за участие и стикер в твой буклет";
                activity.Text = text;
                await stepContext.Context.SendActivityAsync(activity, cancellationToken);
                
                text = "Если по итогам дня войдешь в ТОП-10 участников квиза, подарю супер-приз. Победителей наградим в 16-10 на нашем стенде на третьем этаже.";
                activity.Text = text;
                await stepContext.Context.SendActivityAsync(activity, cancellationToken);
                
                text = "Приходи пообщаться оффлайн.  Угостим кофе, поиграем в True_викторину,  соберем вместе True_конструктор и поговорим про технологии. ";
                activity.Text = text;
                await stepContext.Context.SendActivityAsync(activity, cancellationToken);
                
                text = "Увеличим еще твои шансы на призы?";
                var promptMessage = MessageFactory.Text(text, null, InputHints.ExpectingInput);
                var promptOptions = new PromptOptions
                {
                    Prompt = promptMessage,
                    Choices = new List<Choice>()
                    {
                        new Choice(Constants.TrueLuckyTitle),
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
                    case Constants.TrueLuckyTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueLuckyDialog), new TrueLuckyPersonalData(), cancellationToken);
                    case Constants.TrueEmotionsTitle:
                        return await innerDc.BeginDialogAsync(nameof(TrueEmotionsDialog), null, cancellationToken);
                }
            }

            return null;
        }
    }
}