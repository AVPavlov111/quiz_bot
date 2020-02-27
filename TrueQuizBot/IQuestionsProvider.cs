using System.Threading.Tasks;

namespace TrueQuizBot
{
    public interface IQuestionsProvider
    {
        Task<Question?> GetQuestion(string userId);
        Task<Question?> GetCurrentQuestion(string userId);
    }
}