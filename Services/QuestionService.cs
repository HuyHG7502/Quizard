using Microsoft.EntityFrameworkCore;
using Quizard.Data;
using Quizard.Interfaces;
using Quizard.Models;

namespace Quizard.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly AppDbContext _context;

        public QuestionService(AppDbContext context)
            => _context = context;

        public async Task<IEnumerable<Question>> GetAllAsync()
        {
            return await _context.Questions
                .Include(q => q.Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Question?> GetByIdAsync(Guid id)
        {
            return await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Question> CreateQuestionAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuestionAsync(Guid id)
        {
            var question = await _context.Questions
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Quiz>> GetQuizzesByQuestionAsync(Guid questionId)
        {
            return await _context.Quizzes
                .Where(q => q.QuizQuestions.Any(qn => qn.QuestionId == questionId))
                .ToListAsync();
        }

        public async Task<bool> IsUsedInQuizAsync(Guid questionId)
        {
            return await _context.Quizzes
                .AnyAsync(q => q.QuizQuestions.Any(qn => qn.QuestionId == questionId));
        }
    }
}
