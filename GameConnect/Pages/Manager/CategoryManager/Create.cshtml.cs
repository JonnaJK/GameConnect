using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Data;

namespace GameConnect.Pages.Manager.CategoryManager
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Category Category { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUDl Studio\GameConnect
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Category == null || Category == null)
            {
                return Page();
            }

            _context.Category.Add(Category);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
