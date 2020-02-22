namespace TrueQuizBot
{
    public interface IQuestionsProvider
    {
        Question? GetQuestion(string userId);
    }
}