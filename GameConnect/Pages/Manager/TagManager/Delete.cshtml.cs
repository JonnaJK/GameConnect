using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;
using GameConnect.Domain.Services;

namespace GameConnect.Pages.Manager.TagManager
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
        public Tag Tag { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Tag == null)
            {
                return NotFound();
            }

            var tag = await _context.Tag.FirstOrDefaultAsync(m => m.Id == id);

            if (tag == null)
            {
                return NotFound();
            }
            else
            {
                Tag = tag;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Tag == null)
            {
                return NotFound();
            }
            var tag = await _context.Tag.FindAsync(id);

            if (tag != null)
            {
                Tag = tag;

                var postsWithTag = await _postService.GetPostsByTagAsync(Tag.Id);
                if (postsWithTag != null)
                {
                    foreach (var post in postsWithTag)
                    {
                        post.Tag = null;
                    }
                    await _context.SaveChangesAsync();
                }

                _context.Tag.Remove(Tag);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
