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

        public async Task<int> GetAmountOfUnreadMessages(string userId)
        {
            var unreadMessages = _context.ChatMessageStatus
                .Where(x => x.RecipientId == userId && !x.IsRead)
                .ToList();
            return unreadMessages.Count;
        }
    }
}
