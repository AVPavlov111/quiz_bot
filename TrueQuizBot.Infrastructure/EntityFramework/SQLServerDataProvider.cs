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

        public async Task<List<Winner>> GetWinners(int count)
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var users = await dbContext.GetUsers();
                var winners = users
                    .OrderByDescending(user =>
                        user.AnswerStatistics.Where(stat => stat.IsCorrect)
                            .Sum(stat => stat.PointsNumber)).Take(count).ToList();
                return winners.Select(winner => new Winner()
                {
                    FirstName = winner.PersonalData.FirstName,
                    LastName = winner.PersonalData.LastName,
                    PhoneNumber = winner.PersonalData.PhoneNumber,
                    TotalSum = winner.AnswerStatistics.Where(stat => stat.IsCorrect).Sum(stat => stat.PointsNumber)
                }).ToList();
            });
        }

        public async Task<List<Winner>> GetLuckers()
        {
            return await _contextFactory.RunInTransaction(async dbContext =>
            {
                var users = (await dbContext.GetUsers()).ToArray();
                var count = users.Length;
                var rand = new Random();
                var randomIndexes = new List<int>();
                
                for (var i = 0; i < 3; i++)
                {
                    randomIndexes.Add(rand.Next(0, count-1));
                }

                var winners = randomIndexes.Select(randomIndex => users[randomIndex]).ToList();

                return winners.Select(winner => new Winner()
                {
                    FirstName = winner.PersonalData.FirstName,
                    LastName = winner.PersonalData.LastName,
                    PhoneNumber = winner.PersonalData.PhoneNumber
                }).ToList();
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
                return winners.IndexOf(winners.Single(winner => string.Equals(winner.UserId, userId, StringComparison.InvariantCultureIgnoreCase)));
            });
        }
    }
}