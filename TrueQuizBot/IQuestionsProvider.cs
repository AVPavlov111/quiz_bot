using System.Threading.Tasks;

namespace TrueQuizBot
{
    public interface IQuestionsProvider
    {
        Task<Question?> GetQuestion(string userId);
    }
}