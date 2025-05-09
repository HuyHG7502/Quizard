using Microsoft.EntityFrameworkCore;
using Quizard.Data;
using Quizard.Helpers;
using Quizard.Interfaces;
using Quizard.Models;
using Quizard.Models.Shared;

namespace Quizard.Services
{
    public class QuizService : IQuizService
    {
        private readonly AppDbContext _context;

        public QuizService(AppDbContext context)
            => _context = context;
    
        public async Task<IEnumerable<Quiz>> GetAllAsync()
        {
            return await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Quiz?> GetQuizByIdAsync(Guid id)
        {
            return await _context.Quizzes
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quiz?> GetQuizWithQuestionsAsync(Guid id)
        {
            return await _context.Quizzes
                .Include(q => q.QuizQuestions)
                    .ThenInclude(q => q.Question)
                        .ThenInclude(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<Quiz> CreateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Add(quiz);
            await _context.SaveChangesAsync();
            return quiz;
        }

        public async Task UpdateQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Update(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteQuizAsync(Quiz quiz)
        {
            _context.Quizzes.Remove(quiz);
            await _context.SaveChangesAsync();
        }

        public async Task AddQuestionAsync(Guid quizId, Guid questionId, int? order = null)
        {
            var exists = await _context.QuizQuestions
                .AnyAsync(q => q.QuizId == quizId && q.QuestionId == questionId);
            if (exists) return;
            
            var maxOrder = await _context.QuizQuestions
                .Where(q => q.QuizId == quizId)
                .MaxAsync(q => (int?)q.Order) ?? -1;

            order = order ?? maxOrder + 1;

            _context.Add(new QuizQuestion
            {
                QuizId = quizId,
                QuestionId = questionId,
                Order = order.Value,
            });

            await _context.SaveChangesAsync();
        }

        public async Task RemoveQuestionAsync(Guid quizId, Guid questionId)
        {
            var link = await _context.QuizQuestions
                .FirstOrDefaultAsync(q => q.QuizId == quizId && q.QuestionId == questionId);
            
            if (link != null)
            {
                _context.QuizQuestions.Remove(link);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ReorderQuestionAsync(Guid quizId, Guid questionId, QuestionOrderDirection dir)
        {
            var quiz = await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync(q => q.Id == quizId);

            if (quiz == null) return;

            var quizQuestions = quiz.QuizQuestions.OrderBy(q => q.Order).ToList();
            var index = quizQuestions.FindIndex(q => q.QuestionId == questionId);
            var targetIndex = index + (int)dir;

            if (index < 0 || targetIndex < 0 || targetIndex >= quizQuestions.Count)
                return;

            var q1 = quizQuestions[index];
            var q2 = quizQuestions[targetIndex];

            (q1.Order, q2.Order) = (q2.Order, q1.Order);

            _context.QuizQuestions.UpdateRange(quizQuestions);

            await _context.SaveChangesAsync();
        }
    }
}
