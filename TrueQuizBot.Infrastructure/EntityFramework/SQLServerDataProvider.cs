using System;
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
                var result = user.AnswerStatistics
                    .Where(a => a.IsCorrect || a.IsScipped)
                    .Select(a => a.QuestionIndex)
                    .ToList();
                await dbContext.CommitAsync();
                return result;
            });
        }
        
        public async Task<User> GetUser(string userId)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetOrCreateUser(userId);
                await dbContext.CommitAsync();
                return user;
            });
        }

        public async Task<bool> IsUserAlreadyRegistered(string userId)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user = await dbContext.GetOrCreateUser(userId);
                return user.TrueLuckyPersonalData != null && user.TrueLuckyPersonalData.IsAcceptedPersonalDataProcessing;
            });
        }

        public async Task SaveAnswer(string userId, Question question, string answer, bool isSkipped)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                answer = question.QuestionAboutLanguage ? answer.Replace(" ", "") : answer;
                var user =  await dbContext.GetOrCreateUser(userId);
                user.SaveAnswer(new AnswerStatistic
                {
                    Answer = answer,
                    IsCorrect = question.IsCorrectAnswer(answer),
                    QuestionIndex = question.Index,
                    PointsNumber = question.PointsNumber,
                    IsScipped = isSkipped
                });
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

        public async Task<List<Winner>> GetWinners(int count)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var users = await dbContext.GetUsers();
                return users
                    .Where(user => user.TrueLuckyPersonalData != null && user.TrueLuckyPersonalData.IsAcceptedPersonalDataProcessing)
                    .OrderByDescending(user => user.AnswerStatistics.Where(stat => stat.IsCorrect).Sum(stat => stat.PointsNumber))
                    .Take(count)
                    .Select(user => new Winner(user))
                    .ToList();
            });
        }

        public async Task<List<Winner>> GetLuckers()
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var users = (await dbContext.GetUsers())
                    .Where(user => user.TrueLuckyPersonalData != null && user.TrueLuckyPersonalData.IsAcceptedPersonalDataProcessing)
                    .ToArray();
                
                var count = users.Length;
                var rand = new Random();
                var randomIndexes = new List<int>();
                
                for (var i = 0; i < 3; i++)
                {
                    randomIndexes.Add(rand.Next(0, count-1));
                }

                var winners = randomIndexes.Select(randomIndex => users[randomIndex]).ToList();

                return winners.Select(user => new Winner(user)).ToList();
            });
        }

        public async Task<int?> GetCurrentQuestionIndex(string userId)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user =  await dbContext.GetOrCreateUser(userId);
                await dbContext.CommitAsync();
                return user.CurrentQuestionIndex;
            });
        }

        public async Task SaveQurrentQuestionIndex(string userId, int questionIndex)
        {
            await _contextFactory.RunInTransaction(async dbContext =>
            {
                var user =  await dbContext.GetOrCreateUser(userId);
                user.CurrentQuestionIndex = questionIndex;
                await dbContext.CommitAsync();
            });
        }

        public async Task<int> GetCurrentPosition(string userId)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var users = await dbContext.GetUsers();
                var winners = users
                    .OrderByDescending(user =>
                        user.AnswerStatistics.Where(stat => stat.IsCorrect)
                            .Sum(stat => stat.PointsNumber)).ToList();
                return winners.IndexOf(winners.Single(winner => string.Equals(winner.UserId, userId, StringComparison.InvariantCultureIgnoreCase))) + 1;
            });
        }

        public async Task<List<Winner>> GetEmails()
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var users = await dbContext.GetUsers();
                return users.Where(user => user.TrueLuckyPersonalData?.IsAcceptedPersonalDataProcessing ?? false).Select(user => new Winner(user)).ToList();
            });
        }
    }
}