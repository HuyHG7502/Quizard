using Quizard.Pages.Quizzes;

namespace Quizard.Interfaces
{
    public interface IAnswerStore
    {
        Task SaveAsync(QuizAttemptSession session);
        Task<Dictionary<Guid, List<Guid>>?> LoadAsync(Guid attemptId);
        Task RemoveAsync(Guid attemptId);
    }
}
