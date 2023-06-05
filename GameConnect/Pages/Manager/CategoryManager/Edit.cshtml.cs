using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameConnect.DAL;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages.Manager.CategoryManager
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly HttpService _httpService;

        [BindProperty]
        public Category Category { get; set; } = default!;

        public EditModel(ApplicationDbContext context, HttpService httpService)
        {
            _context = context;
            _httpService = httpService;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _httpService.HttpGetRequest<Category>($"category/{id}");
            if (category == null)
            {
                return NotFound();
            }
            Category = category;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            await _httpService.HttpUpdateRequest($"category/{Category.Id}", Category);
            return RedirectToPage("./Index");
        }
    }
}
