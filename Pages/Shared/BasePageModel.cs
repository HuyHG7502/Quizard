using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Quizard.Data;
using Quizard.Models;

namespace Quizard.Pages.Shared
{
    public abstract class BasePageModel : PageModel
    {
        protected readonly AppDbContext _context;

        protected BasePageModel(AppDbContext context)
            => _context = context;

        public User? CurrentUser { get; private set; } = null!;

        /*
        public async Task<bool> GetCurrentUserAsync()
        {
            if (User.Identity?.IsAuthenticated != true) return false;

            var username = User.Identity.Name;
            CurrentUser = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);

            return CurrentUser != null;
        }
        */
    }
}
