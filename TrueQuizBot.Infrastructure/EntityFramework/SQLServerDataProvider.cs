using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TrueQuizBot.Infrastructure.EntityFramework
{
    public class SqlServerDataProvider : IDataProvider
    {
        private readonly DbContextFactory _contextFactory;

        public SqlServerDataProvider(DbContextFactory contextFactory)
        {
            _contextFactory = contextFactory;
        }
        
        public async Task<User> AddUser(string userId)
        {
            var createdUser = await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.FindUser(userId);
                if (user != null)
                {
                    return user;
                }
                
                user = new User(userId);
                dbContext.Add(user);
                await dbContext.CommitAsync();

                return user;
            });
          
            return createdUser;
        }

        public async Task<List<int>> GetCompletedQuestionsIndexes(string userId)
        {
            await using var context = _contextFactory.GetContext();
            var user = await context.GetUser(userId);
            return user.AnswerStatistics.Select(a => a.QuestionIndex).ToList();
        }

        public async Task SaveAnswer(string userId, Question question, string answer)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user =  await dbContext.GetUser(userId);
                user.SaveAnswer(new AnswerStatistic
                {
                    Answer = answer,
                    IsCorrect = question.IsCorrectAnswer(answer),
                    QuestionIndex = question.Index,
                    PointsNumber = question.PointsNumber
                });
                await dbContext.CommitAsync();
            });
        }

        public async Task ClearAnswerStatistic(string userId)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetUser(userId);
                user.ClearAnswerStatistic();
                await dbContext.CommitAsync();
            });
        }

        public async Task SavePersonalData(string userId, PersonalData personalData)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetUser(userId);
                user.SavePersonalData(personalData);
                await dbContext.CommitAsync();
            });
        }

        public async Task<bool> IsUserAlreadyEnterPersonalData(string userId)
        {
            await using var context = _contextFactory.GetContext();
            var user = await context.GetUser(userId);
            return user.PersonalData != null && user.PersonalData.IsAcceptedPersonalDataProcessing;
        }
    }
}