namespace TrueQuizBot
{
    public class PersonalData
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set;}
        public string? PhoneNumber { get;  set;}
        public string? Email { get;  set;}
        
        public bool IsAcceptedPersonalDataProcessing { get;  set;}

        public void Update(PersonalData personalData)
        {

            FirstName = personalData.FirstName;
            LastName = personalData.LastName;
            PhoneNumber = personalData.PhoneNumber;
            Email = personalData.Email;
            IsAcceptedPersonalDataProcessing = personalData.IsAcceptedPersonalDataProcessing;

        }
    }
}