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
            PersonalData.Update(personalData);
        }
    }
}