using System.Linq;
using JetBrains.Annotations;

namespace TrueQuizBot
{
    [PublicAPI]
    public class Winner
    {
        public Winner(User user)
        {
            DisplayName = user.TrueLuckyPersonalData!.DisplayName!;
            PhoneNumber = user.TrueLuckyPersonalData!.PhoneNumber!;
            EmailAddress = user.TrueLuckyPersonalData!.EmailAddress!;
            TotalSum = user.AnswerStatistics.Where(stat => stat.IsCorrect).Sum(stat => stat.PointsNumber);
        }

        public string DisplayName { get; }
        public string PhoneNumber { get;  }
        public int TotalSum { get; }
        public string EmailAddress { get; set; }
    }
}