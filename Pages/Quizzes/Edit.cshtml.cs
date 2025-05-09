using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;
using Quizard.Models.Shared;
using Quizard.ViewModels;
using System.Diagnostics;

namespace Quizard.Pages.Quizzes
{
    public class EditModel : PageModel
    {
        private readonly IQuizService _quizService;
        private readonly IQuestionService _questionService;

        public EditModel(IQuizService quizService, IQuestionService questionService)
            => (_quizService, _questionService) = (quizService, questionService);

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public QuizViewModel QuizVm { get; set; } = new();

        public List<Question> QuestionBank { get; set; } = [];

        public async Task<IActionResult> OnGetAsync()
        {
            var quiz = await _quizService.GetQuizWithQuestionsAsync(Id);
            if (quiz == null)
                return NotFound();

            QuizVm = new QuizViewModel
            {
                Id = quiz.Id,
                Title = quiz.Title,
                Description = quiz.Description,
                TimeLimit = quiz.TimeLimit?.TotalMinutes,
                IsRandomOrder = quiz.IsRandomOrder,
                Questions = quiz.QuizQuestions
                    .OrderBy(qq => qq.Order)
                    .Select(qq => new QuestionViewModel
                    {
                        QuestionId = qq.QuestionId,
                        Text = qq.Question.Text,
                        Order = qq.Order,
                        IsMultiSelect = qq.Question.IsMultiSelect,
                    })
                    .ToList()
            };

            QuestionBank = [.. (await _questionService.GetAllAsync())];

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var quiz = await _quizService.GetQuizByIdAsync(QuizVm.Id);
            if (quiz == null)
                return NotFound();

            quiz.Title = QuizVm.Title;
            quiz.Description = QuizVm.Description;
            quiz.TimeLimit = QuizVm.TimeLimit.HasValue
                ? TimeSpan.FromMinutes(QuizVm.TimeLimit.Value)
                : null;
            quiz.IsRandomOrder = QuizVm.IsRandomOrder;

            await _quizService.UpdateQuizAsync(quiz);

            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostAddQuestionAsync(Guid questionId)
        {
            await _quizService.AddQuestionAsync(Id, questionId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveQuestionAsync(Guid questionId)
        {
            await _quizService.RemoveQuestionAsync(Id, questionId);
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostReorderQuestionAsync(Guid questionId, QuestionOrderDirection dir)
        {
            await _quizService.ReorderQuestionAsync(Id, questionId, dir);
            return RedirectToPage(new { id = Id });
        }
    }
}
