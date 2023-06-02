using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Data;
using GameConnect.DAL;

namespace GameConnect.Pages.Manager.CategoryManager
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpService _httpService;

        public CreateModel(ApplicationDbContext context, HttpService httpService)
        {
            _context = context;
            _httpService = httpService;
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
            if (!ModelState.IsValid /*|| _context.Category == null*/ || Category == null)
            {
                return Page();
            }

            await _httpService.HttpPostRequest<Category>("category/" + Category.Name, Category);
            //_context.Category.Add(Category);
            //await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
