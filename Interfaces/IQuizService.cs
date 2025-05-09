using Quizard.Models;
using Quizard.Models.Shared;

namespace Quizard.Interfaces
{
    public interface IQuizService
    {
        // Get
        Task<IEnumerable<Quiz>> GetAllAsync();
        Task<Quiz?> GetQuizByIdAsync(Guid id);
        Task<Quiz?> GetQuizWithQuestionsAsync(Guid id);

        // Post
        Task<Quiz> CreateQuizAsync(Quiz quiz);

        // Put / Patch
        Task UpdateQuizAsync(Quiz quiz);
        Task AddQuestionAsync(Guid quizId, Guid questionId, int? order = null);
        Task RemoveQuestionAsync(Guid quizId, Guid questionId);
        Task ReorderQuestionAsync(Guid quizId, Guid questionId, QuestionOrderDirection dir);

        // Delete
        Task DeleteQuizAsync(Quiz quiz);
    }
}
