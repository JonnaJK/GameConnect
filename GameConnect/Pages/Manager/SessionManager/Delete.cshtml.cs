using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;

namespace GameConnect.Pages.Manager.SessionManager
{
    public class DeleteModel : PageModel
    {
        private readonly GameForum_Inlämningsuppgift.Data.ApplicationDbContext _context;

        public DeleteModel(GameForum_Inlämningsuppgift.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Session Session { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }

            var session = await _context.Session.FirstOrDefaultAsync(m => m.Id == id);

            if (session == null)
            {
                return NotFound();
            }
            else
            {
                Session = session;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Session == null)
            {
                return NotFound();
            }
            var session = await _context.Session.FindAsync(id);

            if (session != null)
            {
                Session = session;
                _context.Session.Remove(Session);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
