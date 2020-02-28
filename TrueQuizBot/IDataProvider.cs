using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrueQuizBot
{
    public interface IDataProvider
    {
        Task<List<int>> GetCompletedQuestionsIndexes(string userId);
        Task SaveAnswer(string userId, Question question, string answer, bool isSkipped);
        Task SavePersonalDataFromTrueLucky(string userId, TrueLuckyPersonalData luckyPersonalData);
        Task<List<Winner>> GetWinners(int count);
        Task<List<Winner>> GetLuckers();
        Task<int?> GetCurrentQuestionIndex(string userId);
        Task SaveQurrentQuestionIndex(string userId, int questionIndex);
        Task<int> GetCurrentPosition(string userId);
        Task<List<Winner>> GetEmails();
        Task<bool> IsUserAlreadyRegistered(string userId);
        Task<User> GetUser(string userId);
    }
}