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

        [BindProperty]
        public ChatMessage NewMessage { get; set; } = new();
        public List<ChatMessageStatus> UnreadMessages { get; set; } = new();
        public List<Session>? Sessions { get; set; } = new();
        public List<ChatMessage> ChatMessages { get; set; } = new();
        public User? LoggedInUser { get; set; } = new();
        public bool ShowSettings { get; set; }
        public bool IsSessionCreator { get; set; }
        public bool IsNoMessagesToShow { get; set; }
        public List<int> UnreadMessageInSessionId { get; set; } = new();
        public List<string> ReadBy { get; set; } = new();


        public ChatModel(UserService userService, SessionService sessionService, ChatMessageService chatMessageService, ChatMessageStatusService chatMessageStatusService)
        {
            _userService = userService;
            _sessionService = sessionService;
            _chatMessageService = chatMessageService;
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
                    if (session != null)
                        return RedirectToPage("/Chat", new Session { Id = session.Id });
                }
            }

            if (sessionId != 0)
            {
                ChatMessages = await _chatMessageService.ChatMessagesFromSessionIdAsync(sessionId);
                //await SetReadByText();
            }
            if (session != null)
            {
                if (session.Id != 0)
                {
                    ChatMessages = await _chatMessageService.ChatMessagesFromSessionIdAsync(session.Id);
                    //await SetReadByText();
                }
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
                //await SetReadByText();

                ShowSettings = true;
                var settingSession = await _sessionService.GetSessionAsync(settingsSessionId);

                if (LoggedInUser != null)
                {
                    if (LoggedInUser.Id == settingSession.CreatorId)
                    {
                        IsSessionCreator = true;
                    }
                }
            }
            if (closeSettings)
            {
                ShowSettings = false;
                if (session != null)
                {
                    session.Id = settingsSessionId;
                    RedirectToPage("/Chat", new Session { Id = session.Id });
                }
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

            if (!string.IsNullOrEmpty(NewMessage.Message))
            {
                await _chatMessageService.CreateChatMessageAsync(NewMessage, User);
            }

            return RedirectToPage("/Chat", new Session { Id = NewMessage.SessionId });
        }

        private async Task<List<ChatMessageStatus>> GetUsersUnreadMessages()
        {
            return await _chatMessageStatusService.GetUsersUnreadMessages(LoggedInUser ?? new User { Id = string.Empty });
        }

        private void SortSessionsByUnreadMessages()
        {
            var unreadMessagesInSession = new List<Session>();
            var readMessagesInSession = new List<Session>();
            UnreadMessageInSessionId = new List<int>();
            if (Sessions != null)
            {
                foreach (var session in Sessions)
                {
                    foreach (var unReadMessage in UnreadMessages)
                    {
                        if (unReadMessage.Session != null)
                        {
                            if (session.Id == unReadMessage.Session.Id)
                            {
                                unreadMessagesInSession.Add(session);
                                UnreadMessageInSessionId.Add(session.Id);
                            }
                        }
                    }
                    readMessagesInSession.Add(session);
                }
            }
            //Sessions = new List<Session>();
            //Sessions.AddRange(unreadMessagesInSession);
            //Sessions.AddRange(readMessagesInSession);
        }

        private async Task SetReadByText()
        {
            if (LoggedInUser == null)
                return;

            var lastMessage = ChatMessages.Last(x => x.UserId == LoggedInUser.Id);
            if (lastMessage.Recipients != null)
            {
                foreach (var recipient in lastMessage.Recipients)
                {
                    var status = await _chatMessageStatusService.GetStatusByMessageIdAsync(lastMessage.Id);
                    if (status == null)
                        continue;
                    if (status.IsRead)
                    {
                        ReadBy.Add(recipient.UserName ?? string.Empty);
                    }
                }
            }
        }
    }
}
