using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace Quizard.Pages.Quizzes
{
    public class StartModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly ITakeQuizService _takeQuizService;

        public StartModel(IQuizService quizService, ITakeQuizService takeQuizService)
            => (_quizService, _takeQuizService) = (quizService, takeQuizService);

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        [Display(Name = "Your Name")]
        public string? UserName { get; set; }

        public AttemptViewModel QuizVm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var quiz = await _quizService.GetQuizByIdAsync(Id);
            if (quiz == null)
                return NotFound();

            QuizVm = new AttemptViewModel
            {
                QuizId = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.TimeLimit?.TotalMinutes
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return await OnGetAsync();

            var userName = string.IsNullOrWhiteSpace(UserName) ? "Anonymous" : UserName;
            var attempt = await _takeQuizService.AttemptQuizAsync(Id, userName);
            return RedirectToPage("/Quizzes/Take", new { id = attempt.Id });
        }
    }
}
