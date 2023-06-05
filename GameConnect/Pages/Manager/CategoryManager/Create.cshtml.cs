using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Data;
using GameConnect.DAL;

namespace GameConnect.Pages.Manager.CategoryManager
{
    public class CreateModel : PageModel
    {
        private readonly HttpService _httpService;

        [BindProperty]
        public Category Category { get; set; } = default!;

        public CreateModel(HttpService httpService)
        {
            _httpService = httpService;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || Category == null)
            {
                return Page();
            }

            await _httpService.HttpPostRequest("category/", Category);
            return RedirectToPage("./Index");
        }
    }
}
