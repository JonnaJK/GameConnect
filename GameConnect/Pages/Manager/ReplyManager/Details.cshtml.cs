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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public Reply Reply { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Reply == null)
            {
                return NotFound();
            }

            var reply = await _context.Reply.FirstOrDefaultAsync(m => m.Id == id);
            if (reply == null)
            {
                return NotFound();
            }
            else
            {
                Reply = reply;
            }
            return Page();
        }
    }
}
