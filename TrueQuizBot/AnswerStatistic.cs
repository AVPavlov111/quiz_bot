namespace TrueQuizBot
{
    public class AnswerStatistic
    {
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
        public string? Answer { get; set; }
        public int PointsNumber { get; set; }
    }
}