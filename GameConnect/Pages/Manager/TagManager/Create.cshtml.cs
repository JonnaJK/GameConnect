using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;

namespace GameConnect.Pages.Manager.TagManager
{
    public class CreateModel : PageModel
    {
        private readonly GameForum_Inlämningsuppgift.Data.ApplicationDbContext _context;

        public CreateModel(GameForum_Inlämningsuppgift.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Tag Tag { get; set; } = default!;


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid || _context.Tag == null || Tag == null)
            {
                return Page();
            }

            _context.Tag.Add(Tag);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
