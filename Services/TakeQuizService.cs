using Microsoft.EntityFrameworkCore;
using Quizard.Data;
using Quizard.Helpers;
using Quizard.Interfaces;
using Quizard.Models;
using Quizard.Pages.Quizzes;
using Quizard.ViewModels;

namespace Quizard.Services
{
    public class TakeQuizService : ITakeQuizService
    {
        private readonly AppDbContext _context;
        private readonly IAnswerStore _answerStore;

        public TakeQuizService(AppDbContext context, IAnswerStore answerStore)
            => (_context, _answerStore) = (context, answerStore);

        public async Task<QuizAttempt> AttemptQuizAsync(Guid quizId, string userName)
        {
            var existing = await GetOngoingAttemptAsync(quizId, userName);
            if (existing != null)
                return existing;

            var quiz = await _context.Quizzes
                .Include(q => q.QuizQuestions)
                .FirstOrDefaultAsync(q => q.Id == quizId) ?? throw new InvalidOperationException("Quiz not found");

            var quizQuestions = quiz.IsRandomOrder
                ? quiz.QuizQuestions.Shuffle()
                : quiz.QuizQuestions.OrderBy(q => q.Order).ToList();

            var now = DateTime.UtcNow;

            var attempt = new QuizAttempt
            {
                QuizId = quizId,
                UserName = userName,
                StartedOn = now,
                ExpiresAt = quiz.TimeLimit.HasValue ? now + quiz.TimeLimit.Value : null,
                QuizAnswers = quizQuestions.Select((q, idx) => new QuizAnswer
                {
                    QuestionId = q.QuestionId,
                    Order = idx,
                    AnswerChoices = []
                }).ToList()
            };

            _context.QuizAttempts.Add(attempt);
            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<QuizAttempt> SubmitQuizAsync(Guid attemptId)
        {
            var attempt = await _context.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.QuizQuestions)
                        .ThenInclude(q => q.Question)
                            .ThenInclude(q => q.Choices)
                .Include(a => a.QuizAnswers)
                    .ThenInclude(aa => aa.AnswerChoices)
                .FirstOrDefaultAsync(a => a.Id == attemptId) ?? throw new InvalidOperationException("Attempt not found");
            
            var cached = await _answerStore.LoadAsync(attemptId);
            if (cached != null)
            {
                await SaveAnswersToDbAsync(new QuizAttemptSession
                {
                    AttemptId = attemptId,
                    Answers = cached
                });
                await _answerStore.RemoveAsync(attemptId);
            }

            attempt.CompletedOn = DateTime.UtcNow;
            attempt.Score = CalculateScore(attempt);

            await _context.SaveChangesAsync();
            return attempt;
        }

        public async Task<QuizAttempt?> GetAttemptByIdAsync(Guid attemptId)
        {
            return await _context.QuizAttempts
                .Include(a => a.Quiz)
                    .ThenInclude(q => q.QuizQuestions)
                        .ThenInclude(q => q.Question)
                            .ThenInclude(q => q.Choices)
                .Include(a => a.QuizAnswers)
                    .ThenInclude(qa => qa.Question)
                        .ThenInclude(q => q.Choices)
                .Include(a => a.QuizAnswers)
                    .ThenInclude(qa => qa.AnswerChoices)
                .FirstOrDefaultAsync(a => a.Id == attemptId);
        }

        public async Task<QuizAttempt?> GetOngoingAttemptAsync(Guid quizId, string userName)
        {
            return await _context.QuizAttempts
                .Include(a => a.QuizAnswers)
                    .ThenInclude(q => q.AnswerChoices)
                .FirstOrDefaultAsync(a =>
                    a.QuizId == quizId &&
                    a.UserName == userName &&
                    a.CompletedOn == null);
        }

        public async Task<IEnumerable<QuizAttempt>> GetAllAttemptsAsync()
        {
            return await _context.QuizAttempts
                .Include(a => a.Quiz)
                .OrderByDescending(a => a.StartedOn)
                .ToListAsync();
        }

        public async Task<IEnumerable<QuizAttempt>> GetStaleAttemptsAsync()
        {
            var now = DateTime.UtcNow;
            return await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Where(a => 
                    a.CompletedOn == null &&
                    a.ExpiresAt.HasValue &&
                    a.ExpiresAt <= now)
                .ToListAsync();
        }

        public async Task<DateTime?> GetNextExpiryAsync()
        {
            return await _context.QuizAttempts
                .Where(a => a.CompletedOn == null && a.ExpiresAt.HasValue)
                .OrderBy(a => a.ExpiresAt)
                .Select(a => a.ExpiresAt)
                .FirstOrDefaultAsync();
        }

        public async Task<AttemptViewModel> BuildAttemptViewModelAsync(QuizAttempt attempt)
        {
            var quiz = attempt.Quiz ?? throw new InvalidOperationException("Quiz not found.");

            var cached = await _answerStore.LoadAsync(attempt.Id);

            var questionVms = attempt.QuizAnswers
                .OrderBy(a => a.Order)
                .Select(a =>
                {
                    var question = quiz.QuizQuestions
                        .First(q => q.QuestionId == a.QuestionId).Question;

                    var selectedChoiceIds = cached != null && cached.TryGetValue(a.QuestionId, out var selected)
                        ? selected
                        : a.AnswerChoices.Select(ac => ac.ChoiceId).ToList();

                    return new QuestionViewModel
                    {
                        QuestionId = a.QuestionId,
                        Text = question.Text,
                        Order = a.Order,
                        IsMultiSelect = question.IsMultiSelect,
                        SelectedChoiceIds = selectedChoiceIds,
                        Choices = question.Choices.Select(c => new ChoiceViewModel
                        {
                            ChoiceId = c.Id,
                            Text = c.Text
                        }).ToList(),
                    };
                }).ToList();

            return new AttemptViewModel
            {
                AttemptId = attempt.Id,
                QuizId = quiz.Id,
                UserName = attempt.UserName,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.TimeLimit?.TotalMinutes,
                StartedOn = attempt.StartedOn,
                ExpiresAt = attempt.ExpiresAt,
                Questions = questionVms,
                Answers = cached ?? []
            };
        }

        public async Task<double> CalculateScoreAsync(Guid attemptId)
        {
            var attempt = await GetAttemptByIdAsync(attemptId);
            return attempt == null ? 0.0 : CalculateScore(attempt);

        }

        private double CalculateScore(QuizAttempt attempt)
        {
            if (attempt.Quiz?.QuizQuestions == null || attempt.QuizAnswers == null)
                return 0.0;

            var correctAnswers = 0;

            foreach (var answer in attempt.QuizAnswers)
            {
                var quizQuestion = attempt.Quiz.QuizQuestions
                    .FirstOrDefault(q => q.QuestionId == answer.QuestionId);

                var question = quizQuestion?.Question;

                if (question == null || question.Choices == null) continue;

                var correctChoiceIds = question.Choices
                    .Where(c => c.IsCorrect)
                    .Select(c => c.Id)
                    .OrderBy(id => id)
                    .ToList();

                var selectedChoiceIds = answer.AnswerChoices
                    .Select(ac => ac.ChoiceId)
                    .OrderBy(id => id)
                    .ToList();

                if (correctChoiceIds.SequenceEqual(selectedChoiceIds))
                {
                    correctAnswers++;
                }
            }

            var totalQuestions = attempt.Quiz.QuizQuestions.Count;
            return totalQuestions > 0
                ? Math.Round((double)correctAnswers / totalQuestions * 100, 2)
                : 0.0;
        }

        public async Task SaveAnswersToCacheAsync(QuizAttemptSession session)
        {
            await _answerStore.SaveAsync(session);
        }

        public async Task SaveAnswersToDbAsync(QuizAttemptSession session)
        {
            var (attemptId, answers) = (session.AttemptId, session.Answers);

            var quizAnswers = await _context.QuizAnswers
                .Include(a => a.AnswerChoices)
                .Where(a => a.QuizAttemptId == attemptId)
                .ToListAsync();

            foreach (var (questionId, choiceIds) in answers)
            {
                var answer = quizAnswers.FirstOrDefault(a => a.QuestionId == questionId);
                if (answer == null) continue;

                _context.QuizAnswerChoices.RemoveRange(answer.AnswerChoices);
                answer.AnswerChoices.Clear();

                foreach (var choiceId in choiceIds)
                {
                    answer.AnswerChoices.Add(new QuizAnswerChoice
                    {
                        QuizAnswerId = answer.Id,
                        ChoiceId = choiceId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
