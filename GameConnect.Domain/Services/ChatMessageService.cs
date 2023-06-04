using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services
{

    public class ChatMessageService
    {
        private readonly ApplicationDbContext _context;

        public ChatMessageService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ChatMessage>> ChatMessagesFromSessionIdAsync(int id)
        {
            return await _context.ChatMessage
                .Where(x => x.SessionId == id)
                .Include(x => x.User)
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task CreateChatMessageAsync(ChatMessage message)
        {
            await _context.ChatMessage.AddAsync(message);
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
            var asd = await _context.ChatMessageStatus
                .Include(x => x.ChatMessage)
                .Include(x => x.Session)
                .Where(x => x.RecipientId == user.Id && !x.IsRead)
                .ToListAsync();
            return asd;
        }
    }
}
