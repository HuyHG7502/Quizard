using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Quizard.Interfaces;
using Quizard.Models;

namespace Quizard.Pages.Results
{
    public class IndexModel : PageModel
    {
        private readonly ITakeQuizService _takeQuizService;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; private set; } = 10;
        public int PageCount { get; private set; }

        public IndexModel(ITakeQuizService takeQuizService)
            => _takeQuizService = takeQuizService;

        public List<QuizAttempt> Attempts { get; set; } = [];

        public async Task OnGetAsync()
        {
            Attempts = (await _takeQuizService.GetAllAttemptsAsync()).ToList();

            PageCount = (int)Math.Ceiling((double)Attempts.Count / PageSize);
            PageIndex = Math.Max(1, Math.Min(PageIndex, PageCount));
            Attempts = Attempts
                .Skip((PageIndex - 1) * PageSize)
                .Take(PageSize)
                .ToList();
        }
    }
}
