using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;
using Quizard.ViewModels;

namespace Quizard.Pages.Quizzes
{
    public class CreateModel : PageModel
    {
        private readonly IQuizService _quizService;

        public CreateModel(IQuizService quizService)
            => _quizService = quizService;

        [BindProperty]
        public QuizViewModel QuizVm { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var quiz = new Quiz
            {
                Title = QuizVm.Title,
                Description = QuizVm.Description,
                TimeLimit = QuizVm.TimeLimit.HasValue
                    ? TimeSpan.FromMinutes(QuizVm.TimeLimit.Value)
                    : null,
                IsRandomOrder = QuizVm.IsRandomOrder
            };

            await _quizService.CreateQuizAsync(quiz);
            return RedirectToPage("Index");
        }
    }
}
