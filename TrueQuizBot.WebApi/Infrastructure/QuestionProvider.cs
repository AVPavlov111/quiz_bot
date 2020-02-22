using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TrueQuizBot.WebApi.Infrastructure
{
    public class QuestionProvider : IQuestionsProvider
    {
        private readonly IDataProvider _dataProvider;
        private readonly List<Question> _questions;

        public QuestionProvider(IDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
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
            
            var questionIndex = range.ElementAt(0);
            return _questions.FirstOrDefault(q => q.Index == questionIndex);
        }

        private List<Question> LoadQuestions()
        {
            const string questionsPath = "TrueQuizBot.WebApi.Questions.json";

            using var stream = GetType().Assembly.GetManifestResourceStream(questionsPath);
            Debug.Assert(stream != null, nameof(stream) + " != null");
            using var reader = new StreamReader(stream);
            var questionsStr = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<QuestionsContainer>(questionsStr);
            return result.Questions.Take(1).ToList();
        }

        private class QuestionsContainer
        {
            public List<Question> Questions { get; [UsedImplicitly] set; } = default!;
        }
    }
}