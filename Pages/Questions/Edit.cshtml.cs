using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.ViewModels;

namespace Quizard.Pages.Questions
{
    public class EditModel : PageModel
    {
        private readonly IQuestionService _questionService;

        public EditModel(IQuestionService questionService)
            => _questionService = questionService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public QuestionFormViewModel QuestionVm { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var question = await _questionService.GetByIdAsync(Id);
            if (question == null) return NotFound();

            QuestionVm = new QuestionFormViewModel
            {
                Id = question.Id,
                Text = question.Text,
                IsMultiSelect = question.IsMultiSelect,
                Choices = question.Choices.Select(c => new ChoiceFormViewModel
                {
                    Id = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid) return Page();

            if (!QuestionVm.IsMultiSelect && QuestionVm.Choices.Count(c => c.IsCorrect) > 1)
            {
                ModelState.AddModelError(string.Empty, "Only one correct answer is allowed when multiple selection is disabled.");
                return Page();
            }

            var question = await _questionService.GetByIdAsync(QuestionVm.Id);
            if (question == null) return NotFound();

            question.Text = QuestionVm.Text;
            question.IsMultiSelect = QuestionVm.IsMultiSelect;

            // Update existing choices
            foreach (var vm in QuestionVm.Choices)
            {
                var choice = question.Choices.FirstOrDefault(c => c.Id == vm.Id);
                if (choice != null)
                {
                    choice.Text = vm.Text;
                    choice.IsCorrect = vm.IsCorrect;
                }
            }

            await _questionService.UpdateQuestionAsync(question);
            return RedirectToPage("/Questions/Index");
        }
    }
}
