using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoBot1.Infrastructure
{
    public interface IQuestionsProvider
    {
        Question? GetQuestion(string userId);
    }
    
    public class InMemoryQuestionsProvider : IQuestionsProvider
    {
        private readonly IDataProvider _dataProvider;
        private readonly List<Question> _questions;
        private readonly Random _random;
        private const int QuestionsCount = 5;

        public InMemoryQuestionsProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _random = new Random();
            
            _questions = new List<Question>();

            for (var i = 0; i < QuestionsCount; i++)
            {
                _questions.Add(new Question
                {
                    Index = i,
                    Text = $"Text {i}",
                    Answers = new List<string>
                    {
                        "value 1",
                        "value 2",
                        "value 3",
                        "value 4",
                    },
                    CorrectAnswer = $"value {i%4 + 1}",
                    DescribeAnswer = "DescribeAnswer",
                    QuestionType = QuestionType.Choise
                });
            }
        }

        public Question? GetQuestion(string userId)
        {
            var completedQuestionsIndexes = _dataProvider.GetCompletedQuestionsIndexes(userId);
            var range = Enumerable.Range(0, QuestionsCount).Where(i => !completedQuestionsIndexes.Contains(i));

            var notCompletedQuestionsCount = QuestionsCount - completedQuestionsIndexes.Count;
            if (notCompletedQuestionsCount == 0)
            {
                return null;
            }
            var rangeIndex = _random.Next(notCompletedQuestionsCount);
            var questionIndex = range.ElementAt(rangeIndex);
            return _questions[questionIndex];
        }
    }

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

        public bool IsCorrectAnswer(string answer)
        {
            return string.Equals(CorrectAnswer, answer, StringComparison.OrdinalIgnoreCase);
        }
        
    }

    public enum QuestionType
    {
        Choise,
        TextAnswer
    }
}