using System;
using System.Collections.Generic;
using System.Linq;

namespace TrueQuizBot.Infrastructure
{
    public class InMemoryDataProvider : IDataProvider
    {
        private readonly List<User> _users = new List<User>();

                
        public User AddUser(string userId)
        {
            var user = new User(userId);
            _users.Add(user);
            return user;
        }

        public List<int> GetCompletedQuestionsIndexes(string userId)
        {
            var user = GetUser(userId);
            return user.AnswerStatistics.Select(a => a.QuestionIndex).ToList();
        }

        
        public void SaveAnswer(string userId, Question question, string answer)
        {
            var user = GetUser(userId);
            user.AnswerStatistics.Add(new AnswerStatistic
            {
                Answer = answer,
                IsCorrect = question.IsCorrectAnswer(answer),
                QuestionIndex = question.Index,
                PointsNumber = question.PointsNumber
            });
        }

        public void ClearAnswerStatistic(string userId)
        {
            var user = GetUser(userId);
            user.AnswerStatistics = new List<AnswerStatistic>();
        }

        public void SavePersonalData(string userId, PersonalData personalData)
        {
            var user = GetUser(userId);
            user.PersonalData = personalData;
        }

        public bool IsUserAlreadyEnterPersonalData(string userId)
        {
            var user = GetUser(userId);
            return user.PersonalData != null && user.PersonalData.IsAcceptedPersonalDataProcessing;
        }
        
        private User GetUser(string userId)
        {
            return _users.FirstOrDefault(u => string.Equals(u.UserId, userId, StringComparison.OrdinalIgnoreCase)) 
                   // ReSharper disable once ConstantNullCoalescingCondition
                   ?? AddUser(userId);
        }


    }
}