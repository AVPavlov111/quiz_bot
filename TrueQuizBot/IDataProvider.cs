using System.Collections.Generic;
using System.Threading.Tasks;

namespace TrueQuizBot
{
    public interface IDataProvider
    {
        Task<List<int>> GetCompletedQuestionsIndexes(string userId);
        Task SaveAnswer(string userId, Question question, string answer);
        Task ClearAnswerStatistic(string userId);
        Task SavePersonalData(string userId, PersonalData personalData);
        Task SavePersonalDataFromTrueLucky(string userId, TrueLuckyPersonalData luckyPersonalData);
        Task<bool> IsUserAlreadyEnterPersonalData(string userId);
        Task<List<Winner>> GetWinners(int count);
        Task<List<Winner>> GetLuckers();
        Task<int?> GetCurrentQuestionIndex(string userId);
        Task SaveQurrentQuestionIndex(string userId, int questionIndex);
    }
}