using System;
using System.Collections.Generic;
using System.Linq;

namespace EchoBot1.Infrastructure
{
    public interface IDataProvider
    {
        User AddUser(string userId);
        List<int> GetCompletedQuestionsIndexes(string userId);
        void SaveAnswer(string userId, Question question, string answer);
        void ClearAnswerStatistic(string userId);
        void SavePersonalData(string userId, PersonalData personalData);
        bool IsUserAlreadyEnterPersonalData(string userId);
    }

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
                QuestionIndex = question.Index
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
                ?? AddUser(userId);
        }
    }

    public class AnswerStatistic
    {
        public int QuestionIndex { get; set; }
        public bool IsCorrect { get; set; }
        public string? Answer { get; set; }
    }

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

    public class PersonalData
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        
        public bool IsAcceptedPersonalDataProcessing { get; set; }
    }
}