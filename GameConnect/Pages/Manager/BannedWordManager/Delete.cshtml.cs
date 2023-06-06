using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages.Manager.BannedWordManager
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public BannedWord BannedWord { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.BannedWord == null)
            {
                return NotFound();
            }

            var bannedWord = await _context.BannedWord.FirstOrDefaultAsync(x => x.Id == id);

            if (bannedWord == null)
            {
                return NotFound();
            }
            else
            {
                BannedWord = bannedWord;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.BannedWord == null)
            {
                return Page();
            }
            else
            {
                var word = await _context.BannedWord.FindAsync(id);
                if (word != null)
                {
                    _context.BannedWord.Remove(word);
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage("./Index");
            }
        }
    }
}
