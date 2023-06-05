using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameConnect.Pages
{
    public class ChatModel : PageModel
    {
        private readonly UserService _userService;
        private readonly SessionService _sessionService;
        private readonly ChatMessageService _chatMessageService;
        private readonly ChatMessageStatusService _chatMessageStatusService;
        private readonly ApplicationDbContext _context;

        [BindProperty]
        public ChatMessage NewMessage { get; set; }
        public List<ChatMessageStatus> UnreadMessages { get; set; }
        public List<Session> Sessions { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }
        public User LoggedInUser { get; set; }
        public bool ShowSettings { get; set; }
        public bool IsSessionCreator { get; set; }
        public bool IsNoMessagesToShow { get; set; }
        public List<int> UnreadMessageInSessionId { get; set; }
        //public int AmountOfUnreadMessages { get; set; }


        public ChatModel(UserService userService, SessionService sessionService, ChatMessageService chatMessageService, ApplicationDbContext context, ChatMessageStatusService chatMessageStatusService)
        {
            _userService = userService;
            _sessionService = sessionService;
            _chatMessageService = chatMessageService;
            _context = context;
            _chatMessageStatusService = chatMessageStatusService;
        }

        public async Task<IActionResult> OnGetAsync(int sessionId, Session session, string removeUserId, int settingsSessionId, int deleteSessionId, bool closeSettings, bool showMessages)
        {
            LoggedInUser = await _userService.GetUserAsync(User);
            if (LoggedInUser != null)
            {
                Sessions = await _sessionService.GetSessionsFromUserIdAsync(LoggedInUser);
                UnreadMessages = await GetUsersUnreadMessages();
                SortSessionsByUnreadMessages();
                //AmountOfUnreadMessages = await _chatMessageService.GetAmountOfUnreadMessagesInSession
            }

            if (showMessages && sessionId != 0)
            {
                ChatMessages = await _chatMessageService.ChatMessagesFromSessionIdAsync(sessionId);
                if (ChatMessages == null || ChatMessages.Count == 0)
                {
                    IsNoMessagesToShow = true;
                }
                else
                {
                    await _chatMessageStatusService.SetMessagesAsReadAsync(UnreadMessages.Where(x => x.SessionId == sessionId).ToList());
                    session = await _sessionService.GetSessionAsync(sessionId);
                    return RedirectToPage("/Chat", session);
                }
            }

            if (sessionId != 0)
            {
                ChatMessages = await _chatMessageService.ChatMessagesFromSessionIdAsync(sessionId);
            }
            if (session.Id != 0)
            {
                ChatMessages = await _chatMessageService.ChatMessagesFromSessionIdAsync(session.Id);
            }

            if (!string.IsNullOrEmpty(removeUserId) && settingsSessionId != 0)
            {
                List<string> user = new()
                {
                    removeUserId
                };
                await _sessionService.RemoveParticipantFromSessionAsync(user, settingsSessionId);
                return RedirectToPage("/Chat");
            }

            if (settingsSessionId != 0)
            {
                ChatMessages = await _chatMessageService.ChatMessagesFromSessionIdAsync(settingsSessionId);

                ShowSettings = true;
                var settingSession = await _sessionService.GetSessionAsync(settingsSessionId);

                if (LoggedInUser.Id == settingSession.CreatorId)
                {
                    IsSessionCreator = true;
                }
            }
            if (closeSettings)
            {
                ShowSettings = false;
                session.Id = settingsSessionId;
                RedirectToPage("/Chat", session);
            }


            //if(!string.IsNullOrEmpty(removeUserId) && settingsSessionId != 0)
            //{
            //    await _sessionService.RemoveParticipantFromSessionAsync(removeUserId, settingsSessionId);

            //    session.Id = settingsSessionId;
            //    RedirectToPage("/Chat", session); 
            //}
            if (deleteSessionId != 0)
            {

                await _sessionService.DeleteSessionAsync(deleteSessionId);
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            LoggedInUser = await _userService.GetUserAsync(User);
            if (LoggedInUser != null)
            {
                Sessions = await _sessionService.GetSessionsFromUserIdAsync(LoggedInUser);
            }

            await _chatMessageService.CreateChatMessageAsync(NewMessage, User);

            var session = new Session { Id = NewMessage.SessionId };

            return RedirectToPage("/Chat", session);
        }

        private async Task<List<ChatMessageStatus>> GetUsersUnreadMessages()
        {
            return await _chatMessageStatusService.GetUsersUnreadMessages(LoggedInUser);
        }

        private void SortSessionsByUnreadMessages()
        {
            var unreadMessagesInSession = new List<Session>();
            var readMessagesInSession = new List<Session>();
            UnreadMessageInSessionId = new List<int>();

            foreach (var session in Sessions)
            {
                foreach (var unReadMessage in UnreadMessages)
                {
                    if (session.Id == unReadMessage.Session.Id)
                    {
                        unreadMessagesInSession.Add(session);
                        UnreadMessageInSessionId.Add(session.Id);
                    }
                }
                readMessagesInSession.Add(session);
            }
            //Sessions = new List<Session>();
            //Sessions.AddRange(unreadMessagesInSession);
            //Sessions.AddRange(readMessagesInSession);
        }
    }
}
