namespace TrueQuizBot.Infrastructure
{
    public interface IQuestionsProvider
    {
        Question? GetQuestion(string userId);
    }
}