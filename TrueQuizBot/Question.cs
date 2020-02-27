using System;
using System.Collections.Generic;

namespace TrueQuizBot
{
    public class Question
    {
        public int Index { get; set; }
        public string? Text { get; set; }
        public string? ImageUrl { get; set; }

        public List<string>? Answers { get; set; }

        public string? CorrectAnswer { get; set; }

        public string? DescribeAnswerImageUrl { get; set; }
        public string? DescribeAnswer { get; set; }
        public QuestionType QuestionType { get; set; }
        public int PointsNumber { get; set; }

        public bool IsCorrectAnswer(string answer)
        {
            return string.Equals(CorrectAnswer, answer.Trim(), StringComparison.InvariantCultureIgnoreCase);
        }
    }
}