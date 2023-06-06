using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages.Manager.BannedWordManager
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<BannedWord> BannedWords { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.BannedWord != null)
            {
                BannedWords = await _context.BannedWord.ToListAsync();
            }
        }
    }
}
