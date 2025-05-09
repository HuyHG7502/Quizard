using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;

namespace Quizard.Pages.Questions
{
    public class DeleteModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public DeleteModel(IQuestionService questionService)
            => _questionService = questionService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public bool IsInUse { get; set; }

        public Question? Question { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Question = await _questionService.GetByIdAsync(Id);
            if (Question == null)
                return NotFound();

            IsInUse = await _questionService.IsUsedInQuizAsync(Id);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (await _questionService.IsUsedInQuizAsync(Id))
            {
                ModelState.AddModelError(string.Empty, "This question is part of a quiz and cannot be deleted.");
                Question = await _questionService.GetByIdAsync(Id);
                IsInUse = true;

                return Page();
            }

            await _questionService.DeleteQuestionAsync(Id);
            return RedirectToPage("/Questions/Index");
        }
    }
}
