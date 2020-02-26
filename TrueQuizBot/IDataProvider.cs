using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrueQuizBot
{
    public interface IDataProvider
    {
        Task<User> AddUser(string userId);
        Task<List<int>> GetCompletedQuestionsIndexes(string userId);
        Task SaveAnswer(string userId, Question question, string answer);
        Task ClearAnswerStatistic(string userId);
        Task SavePersonalData(string userId, PersonalData personalData);
        Task<bool> IsUserAlreadyEnterPersonalData(string userId);
    }
}