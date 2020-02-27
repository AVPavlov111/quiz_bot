using System.Collections.Generic;

namespace TrueQuizBot
{
    public class User
    {
        public User(string userId)
        {
            UserId = userId;
            AnswerStatistics = new List<AnswerStatistic>();
        }
        public string UserId { get; }
        public List<AnswerStatistic> AnswerStatistics { get; set; }
        public PersonalData? PersonalData { get; set; }
        public TrueLuckyPersonalData? TrueLuckyPersonalData { get; set; }
        
        public int? CurrentQuestionIndex { get; set; }

        public void SaveAnswer(AnswerStatistic answerStatistic)
        {
            AnswerStatistics.Add(answerStatistic);
        }

        public void ClearAnswerStatistic()
        {
            AnswerStatistics.Clear();
        }

        public void SavePersonalData(PersonalData personalData)
        {
            if (PersonalData == null)
            {
                PersonalData = new PersonalData();
            }
            PersonalData.Update(personalData);
        }

        public void SaveTrueLuckyPersonalData(TrueLuckyPersonalData luckyPersonalData)
        {
            if (TrueLuckyPersonalData == null)
            {
                TrueLuckyPersonalData = new TrueLuckyPersonalData();
            }
            TrueLuckyPersonalData.Update(luckyPersonalData);
        }
    }
}