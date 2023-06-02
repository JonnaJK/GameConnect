using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameConnect.Domain.Services;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;

namespace GameConnect.Pages.Manager.CategoryManager
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PostService _postService;

        public DeleteModel(ApplicationDbContext context, PostService postService)
        {
            _context = context;
            _postService = postService;
        }

        [BindProperty]
        public Category Category { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }

            var category = await _context.Category.FirstOrDefaultAsync(m => m.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            else
            {
                Category = category;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Category == null)
            {
                return NotFound();
            }
            var category = await _context.Category.FindAsync(id);

            if (category != null)
            {
                Category = category;

                var postsWithCategory = await _postService.GetPostsByCategoryAsync(Category.Id);
                if (postsWithCategory != null)
                {
                    foreach (var post in postsWithCategory)
                    {
                        post.Category = null;
                    }
                    await _context.SaveChangesAsync();
                }
                _context.Category.Remove(Category);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
