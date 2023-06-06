using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace GameConnect.Domain.Services
{
    public class ChatMessageStatusService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;


        public ChatMessageStatusService(ApplicationDbContext context, UserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task AddMessageAsync(int sessionId, int messageId, ICollection<User> recipients, ClaimsPrincipal currentUser)
        {
            var loggedInUser = await _userService.GetUserAsync(currentUser);
            if (loggedInUser == null)
                return;

            foreach (var recipient in recipients.Where(x => x.Id != loggedInUser.Id))
            {
                ChatMessageStatus message = new ChatMessageStatus()
                {
                    ChatMessageId = messageId,
                    RecipientId = recipient.Id,
                    SessionId = sessionId
                };
                await _context.ChatMessageStatus.AddAsync(message);
            }
            await _context.SaveChangesAsync();
        }
        public async Task<int> GetAmountOfUnreadMessagesInSession(string userId, int sessionId)
        {
            var unreadMessages = await _context.ChatMessageStatus
                .Where(x => x.RecipientId == userId && !x.IsRead)
                .Where(x => x.SessionId == sessionId)
                .ToListAsync();
            return unreadMessages.Count;
        }

        public async Task<int> GetAmountOfUnreadMessages(string userId)
        {
            var unreadMessages = await _context.ChatMessageStatus
                .Where(x => x.RecipientId == userId && !x.IsRead)
                .ToListAsync();
            return unreadMessages.Count;
        }

        public async Task<List<ChatMessageStatus>> GetUsersUnreadMessages(User user)
        {
            return await _context.ChatMessageStatus
                .Include(x => x.Session)
                .Where(x => x.RecipientId == user.Id && !x.IsRead)
                .ToListAsync();
        }

        public async Task SetMessagesAsReadAsync(List<ChatMessageStatus> messageStatuses)
        {
            foreach (var status in messageStatuses)
            {
                status.IsRead = true;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> UsersNumberOfUnreadMessagesAsync(string userId)
        {
            var unreadMessages = await _context.ChatMessageStatus.Where(x => x.RecipientId == userId).ToListAsync();
            return unreadMessages.Where(x => x.IsRead == false).Count();
        }

        public async Task<ChatMessageStatus?> GetStatusByMessageIdAsync(int chatMessageId)
        {
            return await _context.ChatMessageStatus.FirstOrDefaultAsync(x => x.ChatMessageId == chatMessageId);
        }
    }
}
