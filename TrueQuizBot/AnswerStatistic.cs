using System;

namespace TrueQuizBot
{
    public class AnswerStatistic
    {
        public Guid Id { get; set; }
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
        public string? Answer { get; set; }
        public int PointsNumber { get; set; }
        
        public string UserId { get; set; }
        public User User { get; set; }

    }
}