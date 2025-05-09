using Quizard.Models;

namespace Quizard.Interfaces
{
    public interface IQuestionService
    {
        // Get
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question?> GetByIdAsync(Guid id);

        // TODO: Check this
        Task<List<Quiz>> GetQuizzesByQuestionAsync(Guid questionId);

        // Post
        Task<Question> CreateQuestionAsync(Question question);

        // Put / Patch
        Task UpdateQuestionAsync(Question question);

        // Delete
        Task DeleteQuestionAsync(Guid id);

        // Miscellaneous
        Task<bool> IsUsedInQuizAsync(Guid questionId);
    }
}
