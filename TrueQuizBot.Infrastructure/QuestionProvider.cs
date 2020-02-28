using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace TrueQuizBot.Infrastructure
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
        
        public async Task<Question?> GetQuestion(string userId)
        {
            var completedQuestionsIndexes = await _dataProvider.GetCompletedQuestionsIndexes(userId);
            var currentQuestionIndex = await _dataProvider.GetCurrentQuestionIndex(userId);
            
            var incompletedQuestions = _questions.Where(q => !completedQuestionsIndexes.Contains(q.Index)).ToList();

            if (incompletedQuestions.Any() == false)
            {
                return null;
            }

            var resultQuestion = incompletedQuestions.FirstOrDefault(q => q.Index >= currentQuestionIndex);
            // ReSharper disable once ConstantNullCoalescingCondition
            return resultQuestion ?? incompletedQuestions.First();
        }

        public async Task<Question?> GetCurrentQuestion(string userId)
        {
            var index = await _dataProvider.GetCurrentQuestionIndex(userId);
            return index == null ? null : _questions.First(q => q.Index == index);
        }

        private List<Question> LoadQuestions()
        {
            const string questionsPath = "TrueQuizBot.Infrastructure.Questions.json";

            using var stream = GetType().Assembly.GetManifestResourceStream(questionsPath);
            Debug.Assert(stream != null, nameof(stream) + " != null");
            using var reader = new StreamReader(stream);
            var questionsStr = reader.ReadToEnd();
            var result = JsonConvert.DeserializeObject<QuestionsContainer>(questionsStr);
            return result.Questions.ToList();
        }

        private class QuestionsContainer
        {
            public List<Question> Questions { get; [UsedImplicitly] set; } = default!;
        }
    }
}