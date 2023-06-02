using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services
{
    public class SessionService
    {
        private readonly ApplicationDbContext _context;

        public SessionService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Session>?> GetSessionsFromUserIdAsync(User user)
        {
            // Nedan utkommenterade vet vi att det funkar att hämta alla sessions för en user. Detta är önodigt om koden längst ner fungerar.

            //var sessionIds = await _context.Session
            //    .Include(x => x.Participants.Where(x => x.Id == user.Id))
            //    .ToListAsync();

            //if (!sessionIds.Any())
            //    return null;

            //var sessions = new List<Session>();
            //foreach (var sessionId in sessionIds)
            //{
            //    var session = await _context.Session
            //        .Where(x => x.Id == sessionId.Id)
            //        .Include(x => x.Participants)
            //        .Include(x => x.ChatMessages)
            //        .FirstOrDefaultAsync();

            //    if (session != null)
            //        sessions.Add(session);
            //}
            // return sessions;

            var allSessionsInDatabase = await _context.Session
                    .Include(x => x.Participants)
                    .Include(x => x.ChatMessages)
                    .ToListAsync();
            var AllUsersSessions = allSessionsInDatabase.Where(x => x.Participants.Contains(user)).ToList();
            return AllUsersSessions;
        }

        public async Task<Session> GetSessionAsync(int id)
        {
            return await _context.Session.Include(x => x.Participants).FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Session?> GetSessionByCreatorIdAsync(User user)
        {
            var sessions = await _context.Session
                .Where(x => x.CreatorId == user.Id)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            if (sessions == null)
            {
                return null;
            }

            return sessions.First();
        }

        public async Task RemoveParticipantFromSessionAsync(List<string> userIds, int sessionId) //nya metoder !!! 
        {
            var session = await GetSessionAsync(sessionId);
            session.Participants ??= new List<User>();

            foreach (var userId in userIds)
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Id == userId);
                if (user != null)
                    session.Participants.Remove(user);
            }

            await _context.SaveChangesAsync();
        }

        public async Task AddUserToSessionAsync(List<string> userIds, int sessionId)
        {
            var session = await GetSessionAsync(sessionId);
            session.Participants ??= new List<User>();

            foreach (var userId in userIds)
            {
                var user = await _context.User.FirstOrDefaultAsync(x => x.Id == userId);
                if (user != null)
                    session.Participants.Add(user);
            }
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSessionAsync(int sessionId)
        {
            var session = await GetSessionAsync(sessionId);
            _context.Session.Remove(session);
            await _context.SaveChangesAsync();
        }
    }
}
