using System.Collections.Generic;
using System.Linq;

namespace TrueQuizBot.Infrastructure.EntityFramework
{
    public class SqlServerDataProvider : IDataProvider
    {
        private readonly TrueQuizBotDbContext _context;

        public SqlServerDataProvider(TrueQuizBotDbContext context)
        {
            _context = context;
        }
        public User AddUser(string userId)
        {
            var user = new User(userId);
            _context.AddUser(user);
            return user;
        }

        public List<int> GetCompletedQuestionsIndexes(string userId)
        {
            var user = _context.GetUser(userId);
            return user.AnswerStatistics.Select(a => a.QuestionIndex).ToList();
        }

        public void SaveAnswer(string userId, Question question, string answer)
        {
            var user = _context.GetUser(userId);
            user.SaveAnswer(new AnswerStatistic
            {
                Answer = answer,
                IsCorrect = question.IsCorrectAnswer(answer),
                QuestionIndex = question.Index,
                PointsNumber = question.PointsNumber
            });
            _context.SaveChangesAsync();
        }

        public void ClearAnswerStatistic(string userId)
        {
            var user = _context.GetUser(userId);
            user.ClearAnswerStatistic();
            _context.SaveChanges();
        }

        public void SavePersonalData(string userId, PersonalData personalData)
        {
            var user = _context.GetUser(userId);
            user.SavePersonalData(personalData);
            _context.SaveChanges();
        }

        public bool IsUserAlreadyEnterPersonalData(string userId)
        {
            var user = _context.GetUser(userId);
            return user.PersonalData != null && user.PersonalData.IsAcceptedPersonalDataProcessing;
        }
    }
}