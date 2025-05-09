using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;
using Quizard.ViewModels;

namespace Quizard.Pages.Quizzes
{
    public class ReviewModel : PageModel
    {
        private readonly ITakeQuizService _takeQuizService;

        public ReviewModel(ITakeQuizService takeQuizService)
            => _takeQuizService = takeQuizService;

        public QuizAttempt? Attempt { get; set; }

        public List<AnswerViewModel> AnswerVms { get; set; } = [];

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Attempt = await _takeQuizService.GetAttemptByIdAsync(Id);

            if (Attempt == null)
                return NotFound();

            foreach (var answer in Attempt.QuizAnswers.OrderBy(a => a.Order))
            {
                var selectedIds = answer.AnswerChoices
                    .Select(ac => ac.ChoiceId)
                    .ToList();

                var correctIds = answer.Question?.Choices
                    .Where(c => c.IsCorrect)
                    .Select(c => c.Id)
                    .ToList() ?? [];

                bool isCorrect = new HashSet<Guid>(selectedIds).SetEquals(correctIds);

                AnswerVms.Add(new AnswerViewModel
                {
                    QuizAnswer = answer,
                    IsCorrect = isCorrect
                });
            }

            return Page();
        }
    }
}
