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

        public async Task<List<int>> GetCompletedQuestionsIndexes(string userId)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetOrCreateUser(userId);
                var result = user.AnswerStatistics.Select(a => a.QuestionIndex).ToList();
                await dbContext.CommitAsync();
                return result;
            });
        }

        public async Task SaveAnswer(string userId, Question question, string answer)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user =  await dbContext.GetOrCreateUser(userId);
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
                var user = await dbContext.GetOrCreateUser(userId);
                user.ClearAnswerStatistic();
                await dbContext.CommitAsync();
            });
        }

        public async Task SavePersonalData(string userId, PersonalData personalData)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetOrCreateUser(userId);
                user.SavePersonalData(personalData);
                await dbContext.CommitAsync();
            });
        }

        public async Task SavePersonalDataFromTrueLucky(string userId, TrueLuckyPersonalData luckyPersonalData)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetOrCreateUser(userId);
                user.SaveTrueLuckyPersonalData(luckyPersonalData);
                await dbContext.CommitAsync();
            });
        }

        public async Task<bool> IsUserAlreadyEnterPersonalData(string userId)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user =  await dbContext.GetOrCreateUser(userId);
                await dbContext.CommitAsync();
                return user.PersonalData != null && user.PersonalData.IsAcceptedPersonalDataProcessing;
            });
        }
    }
}