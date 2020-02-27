using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

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
            AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
            {
                FirstNameStep,
                LastNameStep,
                PhoneNumberStep,
                EmailStep,
                AcceptanceStep,
                FinalStep
            }));

            InitialDialogId = nameof(WaterfallDialog);
        }
        
        private async Task<DialogTurnResult> FirstNameStep(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var personalData = (PersonalData)stepContext.Options;

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

                return await stepContext.EndDialogAsync(personalData, cancellationToken);
            }

            return await stepContext.EndDialogAsync(null, cancellationToken);
        }
    }
}