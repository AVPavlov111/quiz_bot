using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TrueQuizBot.Infrastructure
{
    public class QuestionProvider : IQuestionsProvider
    {
        private readonly IDataProvider _dataProvider;
        private readonly List<Question> _questions;
        private readonly Random _random;

        public QuestionProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
            _random = new Random();
            _questions = LoadQuestions();
        }
        
        public Question? GetQuestion(string userId)
        {
            var completedQuestionsIndexes = _dataProvider.GetCompletedQuestionsIndexes(userId);
            var range = Enumerable.Range(0, _questions.Count).Where(i => !completedQuestionsIndexes.Contains(i));

            var notCompletedQuestionsCount = _questions.Count - completedQuestionsIndexes.Count;
            if (notCompletedQuestionsCount == 0)
            {
                return null;
            }
            var rangeIndex = _random.Next(notCompletedQuestionsCount);
            var questionIndex = range.ElementAt(rangeIndex);
            return _questions[questionIndex];
        }

        private List<Question> LoadQuestions()
        {
            const string questionsPath = "TrueQuizBot.Questions.json";

            using var stream = GetType().Assembly.GetManifestResourceStream(questionsPath);
            Debug.Assert(stream != null, nameof(stream) + " != null");
            using var reader = new StreamReader(stream);
            var questionsStr = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<QuestionsContainer>(questionsStr);
            return result.Questions;
        }

        private class QuestionsContainer
        {
            public List<Question> Questions { get; [UsedImplicitly] set; } = default!;
        }
    }
}