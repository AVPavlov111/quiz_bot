namespace TrueQuizBot.Infrastructure
{
    public class PersonalData
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        
        public bool IsAcceptedPersonalDataProcessing { get; set; }
    }
}