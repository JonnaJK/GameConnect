using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;

namespace GameConnect.Pages.Manager.ReplyManager
{
    public class IndexModel : PageModel
    {
        private readonly GameForum_Inlämningsuppgift.Data.ApplicationDbContext _context;

        public IndexModel(GameForum_Inlämningsuppgift.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Reply> Reply { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Reply != null)
            {
                Reply = await _context.Reply
                .Include(r => r.Post)
                .Include(r => r.User).ToListAsync();
            }
        }
    }
}
