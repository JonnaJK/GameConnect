using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameConnect.Pages.Manager.BannedWordManager
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public BannedWord NewBannedWord { get; set; } = default!;


        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || NewBannedWord == null)
            {
                return Page();
            }
            else
            {
                await _context.BannedWord.AddAsync(NewBannedWord);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
        }
    }
}
