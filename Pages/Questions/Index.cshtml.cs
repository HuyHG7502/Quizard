using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;

namespace Quizard.Pages.Questions
{
    public class IndexModel : PageModel
    {
        private readonly IQuestionService _questionService;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; private set; } = 6;
        public int PageCount { get; private set; }

        public List<Question> Questions { get; set; } = [];

        public IndexModel(IQuestionService questionService)
            => _questionService = questionService;

        public async Task OnGetAsync()
        {
            Questions = (await _questionService.GetAllAsync()).ToList();
        
            PageCount = (int)Math.Ceiling((double)Questions.Count / PageSize);
            PageIndex = Math.Max(1, Math.Min(PageIndex, PageCount));
            Questions = Questions
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
