using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Helpers;
using Quizard.Interfaces;
using Quizard.ViewModels;
using System.Threading.Tasks;

namespace Quizard.Pages.Quizzes
{
    public class QuizAttemptSession
    {
        public Guid AttemptId { get; set; }
        public Dictionary<Guid, List<Guid>> Answers { get; set; } = [];
    }

    public class TakeModel : PageModel
    {
        private readonly ITakeQuizService _takeQuizService;

        public TakeModel(ITakeQuizService takeQuizService)
            => _takeQuizService = takeQuizService;

        [BindProperty]
        public AttemptViewModel AttemptVm { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [ViewData]
        public int? RemainingTime { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var attempt = await _takeQuizService.GetAttemptByIdAsync(Id);
            if (attempt == null)
                return NotFound();

            // If already completed, redirect to result page
            if (attempt.CompletedOn.HasValue)
            {
                return RedirectToPage("/Quizzes/Review", new { id = Id });
            }

            // If expired, auto-submit the quiz
            var now = DateTime.UtcNow;
            if (attempt.ExpiresAt.HasValue && now >= attempt.ExpiresAt.Value)
            {
                await _takeQuizService.SubmitQuizAsync(attempt.Id);
                return RedirectToPage("/Quizzes/Review", new { id = Id });
            }

            AttemptVm = await _takeQuizService.BuildAttemptViewModelAsync(attempt);

            RemainingTime = AttemptVm.ExpiresAt.HasValue
                ? (int)Math.Max(0, (AttemptVm.ExpiresAt.Value - now).TotalSeconds)
                : null;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var attempt = await _takeQuizService.GetAttemptByIdAsync(Id);
            if (attempt == null)
                return NotFound();

            // Extract from AttemptVm
            var answers = AttemptVm.Questions
                .Where(q => q.SelectedChoiceIds.Any())
                .ToDictionary(q => q.QuestionId, q => q.SelectedChoiceIds);

            await _takeQuizService.SaveAnswersToCacheAsync(new QuizAttemptSession
            {
                AttemptId = Id,
                Answers = answers
            });

            var submitted = await _takeQuizService.SubmitQuizAsync(Id);

            return RedirectToPage("/Quizzes/Review", new { id = submitted.Id });
        }

        public async Task<IActionResult> OnPostSaveAnswersAsync([FromBody] QuizAttemptSession payload)
        {
            if (payload == null || payload.AttemptId == Guid.Empty)
                return BadRequest("Invalid Request");

            await _takeQuizService.SaveAnswersToCacheAsync(payload);
            return new JsonResult(new { success = true });
        }
    }
}
