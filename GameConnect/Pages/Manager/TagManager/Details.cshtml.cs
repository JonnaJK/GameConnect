using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;

namespace GameConnect.Pages.Manager.TagManager
{
    public class DetailsModel : PageModel
    {
        private readonly GameForum_Inlämningsuppgift.Data.ApplicationDbContext _context;

        public DetailsModel(GameForum_Inlämningsuppgift.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
