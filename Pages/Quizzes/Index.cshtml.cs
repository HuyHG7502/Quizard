using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;

namespace Quizard.Pages.Quizzes
{
    public class IndexModel : PageModel
    {
        private readonly IQuizService _quizService;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; private set; } = 6;
        public int PageCount { get; private set; }

        public List<Quiz> Quizzes { get; set; } = [];

        public IndexModel(IQuizService quizService)
            => _quizService = quizService;

        public async Task OnGetAsync()
        {
            Quizzes = (await _quizService.GetAllAsync()).ToList();

            PageCount = (int)Math.Ceiling((double)Quizzes.Count / PageSize);
            PageIndex = Math.Max(1, Math.Min(PageIndex, PageCount));
            Quizzes = Quizzes
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
