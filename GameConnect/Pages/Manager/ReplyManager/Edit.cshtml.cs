using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages.Manager.ReplyManager
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
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
            Reply = reply;
            ViewData["PostId"] = new SelectList(_context.Post, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.User, "Id", "Id");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Reply).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReplyExists(Reply.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ReplyExists(int id)
        {
            return (_context.Reply?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
