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

        public async Task SetMessagesAsReadAsync(List<int> messageIds, string userId)
        {
            var messages = await _context.ChatMessageStatus.Where(x => x.RecipientId == userId).ToListAsync();

            foreach (var id in messageIds)
            {
                foreach (var message in messages.Where(x => x.Id == id))
                {
                    message.IsRead = true;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task<int> UsersNumberOfUnreadMessagesAsync(string userId)
        {
            var unreadMessages = await _context.ChatMessageStatus.Where(x => x.RecipientId == userId).ToListAsync();
            return unreadMessages.Where(x => x.IsRead == false).Count();
        }

    }
}
