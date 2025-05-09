using Quizard.Models;
using Quizard.Pages.Quizzes;
using Quizard.ViewModels;

namespace Quizard.Interfaces
{
    public interface ITakeQuizService
    {
        Task<QuizAttempt> AttemptQuizAsync(Guid quizId, string userName);
        Task<QuizAttempt> SubmitQuizAsync(Guid attemptId);
        Task<QuizAttempt?> GetAttemptByIdAsync(Guid attemptId);
        Task<QuizAttempt?> GetOngoingAttemptAsync(Guid quizId, string userName);
        Task<IEnumerable<QuizAttempt>> GetAllAttemptsAsync();
        Task<IEnumerable<QuizAttempt>> GetStaleAttemptsAsync();
        Task<DateTime?> GetNextExpiryAsync();
        Task<double> CalculateScoreAsync(Guid attemptId);
        Task<AttemptViewModel> BuildAttemptViewModelAsync(QuizAttempt attempt);
        Task SaveAnswersToCacheAsync(QuizAttemptSession session);
        Task SaveAnswersToDbAsync(QuizAttemptSession session);
    }
}
