namespace TrueQuizBot
{
    public class AnswerStatistic
    {
        public long Id { get; set; }
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
        public string? Answer { get; set; }
        public int PointsNumber { get; set; }
        
        public string UserId { get; set; } = default!;
        public User User { get; set; } = null!;

    }
}