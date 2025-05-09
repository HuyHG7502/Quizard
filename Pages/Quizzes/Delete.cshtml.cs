using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;

namespace Quizard.Pages.Quizzes
{
    public class DeleteModel : PageModel
    {
        private readonly IQuizService _quizService;

        public DeleteModel(IQuizService quizService)
            => _quizService = quizService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public Quiz? Quiz { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Quiz = await _quizService.GetQuizByIdAsync(Id);
            if (Quiz == null) return NotFound();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var quiz = await _quizService.GetQuizByIdAsync(Id);
            if (quiz == null) return NotFound();

            await _quizService.DeleteQuizAsync(quiz);
            return RedirectToPage("Index");
        }
    }
}
