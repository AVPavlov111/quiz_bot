using System.Collections.Generic;

namespace TrueQuizBot.Infrastructure
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
        
    }
}