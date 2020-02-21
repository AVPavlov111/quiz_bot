using System.Collections.Generic;

namespace TrueQuizBot.Infrastructure
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
}