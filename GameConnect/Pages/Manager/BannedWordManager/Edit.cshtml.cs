using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameConnect.Pages.Manager.BannedWordManager
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public BannedWord EditBannedWord { get; set; } = default!;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if(id != 0)
            {
                var word = await _context.BannedWord.FindAsync(id);
                if(word != null)
                {
                    EditBannedWord = word;
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (EditBannedWord != null && !string.IsNullOrEmpty(EditBannedWord.Title))
            {
                var word = await _context.BannedWord.FindAsync(EditBannedWord.Id);
                if (word != null)
                {
                    word.Title = EditBannedWord.Title;
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
            }

            return Page();
        }
    }
}
