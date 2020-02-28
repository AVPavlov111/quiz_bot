namespace TrueQuizBot
{
    public class TrueLuckyPersonalData
    {
        public long Id { get; set; }
        public string? DisplayName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? CompanyName { get; set; }
        public string? Position { get; set; }
        public string? Interests { get; set; }
        public string? EmailAddress { get; set; }
        
        public bool IsAcceptedPersonalDataProcessing { get; set; }

        public string UserId { get; set; } = default!;
        public User User { get; set; } = null!;

        public void Update(TrueLuckyPersonalData luckyPersonalData)
        {
            DisplayName = luckyPersonalData.DisplayName;
            PhoneNumber = luckyPersonalData.PhoneNumber;
            CompanyName = luckyPersonalData.CompanyName;
            Position = luckyPersonalData.Position;
            Interests = luckyPersonalData.Interests;
            EmailAddress = luckyPersonalData.EmailAddress;
            IsAcceptedPersonalDataProcessing = luckyPersonalData.IsAcceptedPersonalDataProcessing;
        }
    }
}