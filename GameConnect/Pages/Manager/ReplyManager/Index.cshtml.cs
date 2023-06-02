using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages.Manager.ReplyManager
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
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
