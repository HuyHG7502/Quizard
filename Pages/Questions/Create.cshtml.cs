using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;
using Quizard.ViewModels;

namespace Quizard.Pages.Questions
{
    public class CreateModel : PageModel
    {
        private readonly IQuestionService _questionService;
        public CreateModel(IQuestionService questionService)
            => _questionService = questionService;

        [BindProperty]
        public QuestionFormViewModel QuestionVm { get; set; } = new();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (!QuestionVm.Choices.Any(c => c.IsCorrect))
            {
                ModelState.AddModelError(string.Empty, "Please mark at least one correct answer.");
                return Page();
            }

            if (!QuestionVm.IsMultiSelect && QuestionVm.Choices.Count(c => c.IsCorrect) > 1)
            {
                ModelState.AddModelError(string.Empty, "Only one correct answer is allowed when multiple selection is disabled.");
                return Page();
            }

            var question = new Question
            {
                Text = QuestionVm.Text,
                IsMultiSelect = QuestionVm.IsMultiSelect,
                Choices = QuestionVm.Choices.Select(c => new Choice
                {
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            await _questionService.CreateQuestionAsync(question);
            return RedirectToPage("/Questions/Index");
        }
    }
}
