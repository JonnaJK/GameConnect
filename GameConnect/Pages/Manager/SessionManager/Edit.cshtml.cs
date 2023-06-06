using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GameConnect.Domain.Services;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;

namespace GameConnect.Pages.Manager.SessionManager
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly SessionService _sessionService;

        [BindProperty]
        public Session Session { get; set; } = default!;
        [BindProperty]
        public List<string> AddRecipientIds { get; set; } = new();
        [BindProperty]
        public List<string> RemoveRecipientIds { get; set; } = new();
        public bool IsAdding { get; set; }
        public bool IsRemoving { get; set; }

        public EditModel(ApplicationDbContext context, UserService userService, SessionService sessionService)
        {
            _context = context;
            _userService = userService;
            _sessionService = sessionService;
        }

        public async Task<IActionResult> OnGetAsync(Session? session, int id, int editId)
        {
            if (session != null && session.Id == 0 && id == 0)
            {
                return NotFound();
            }

            if (id != 0)
            {
                session = await _context.Session.Include(x => x.Participants).FirstOrDefaultAsync(m => m.Id == id);
            }
            else
            {
                session = await _context.Session.Include(x => x.Participants).FirstOrDefaultAsync(m => m.Id == (session != null ? session.Id : 0));
            }

            if (editId == 1)
            {
                IsAdding = true;
            }
            else if (editId == 2)
            {
                IsRemoving = true;
            }

            if (session == null)
            {
                return NotFound();
            }
            Session = session;

            //return RedirectToPage("/Manager/SessionManager/Edit", Session);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Session.Participants == null)
                Session.Participants = new List<User>();

            if (AddRecipientIds != null && AddRecipientIds.Count != 0)
            {
                await _sessionService.AddUserToSessionAsync(AddRecipientIds, Session.Id);
            }
            else if (RemoveRecipientIds != null && RemoveRecipientIds.Count != 0)
            {
                await _sessionService.RemoveParticipantFromSessionAsync(RemoveRecipientIds, Session.Id);
            }

            //if (!ModelState.IsValid)
            //{
            //    return Page();
            //}
            //_context.Attach(Session).State = EntityState.Modified;

            return Page();
        }

        private bool SessionExists(int id)
        {
            return (_context.Session?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
