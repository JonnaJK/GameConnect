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
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly ChatMessageService _chatMessageService;
        private readonly UserService _userService;
        private readonly SessionService _sessionService;

        public User LoggedInUser { get; set; }

        [BindProperty]
        public Session Session { get; set; } = default!;
        [BindProperty]
        public ChatMessage NewMessage { get; set; }
        [BindProperty]
        public List<string> RecipientsId { get; set; }
        public string RecipientId { get; set; }
        public string? SendMessageText { get; set; }

        public CreateModel(ApplicationDbContext context, ChatMessageService chatMessageService, UserService userService, SessionService sessionService)
        {
            _context = context;
            _chatMessageService = chatMessageService;
            _userService = userService;
            _sessionService = sessionService;
        }

        public async Task<IActionResult> OnGetAsync(string recipientId, string postText)
        {
            LoggedInUser = await _userService.GetUserAsync(User);

            if (!string.IsNullOrEmpty(recipientId))
            {
                RecipientId = recipientId;
            }
            if (!string.IsNullOrEmpty(postText))
            {
                SendMessageText = postText;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //if (!ModelState.IsValid || _context.Session == null || Session == null)
            //{
            //    return Page();
            //}

            LoggedInUser = await _userService.GetUserAsync(User);
            Session.Participants = new List<User>()
            {
                LoggedInUser
            };
            Session.CreatorId = LoggedInUser.Id;

            foreach (var recipientId in RecipientsId)
            {
                var recipient = await _userService.GetUserAsync(recipientId);
                Session.Participants.Add(recipient);
            }

            _context.Session.Add(Session);
            await _context.SaveChangesAsync();

            var session = await _sessionService.GetSessionByCreatorIdAsync(LoggedInUser);

            NewMessage.SessionId = session.Id;

            await _chatMessageService.CreateChatMessageAsync(NewMessage);

            return RedirectToPage("/Chat");
        }
    }
}
