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
    public class IndexModel : PageModel
    {
        private readonly GameForum_Inlämningsuppgift.Data.ApplicationDbContext _context;

        public IndexModel(GameForum_Inlämningsuppgift.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Tag> Tag { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Tag != null)
            {
                Tag = await _context.Tag.ToListAsync();
            }
        }
    }
}
