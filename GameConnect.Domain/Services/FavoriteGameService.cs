using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services
{
    public class FavoriteGameService
    {
        private readonly ApplicationDbContext _context;
        public FavoriteGameService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ICollection<FavoriteGame>?> GetFavoriteGamesAsync()
        {
            return await _context.FavoriteGame.ToListAsync();
        }

        public async Task<ICollection<FavoriteGame>?> GetUsersFavoriteGamesAsync(User user)
        {
            return await _context.FavoriteGame.Where(x => x.UserId == user.Id).ToListAsync();
        }

        public async Task DeleteFavoriteGameAsync(int id)
        {
            var favoriteGame = _context.FavoriteGame.FirstOrDefault(x => x.Id == id);
            if (favoriteGame != null)
            {
                _context.FavoriteGame.Remove(favoriteGame);
                await _context.SaveChangesAsync();
            }
        }
    }
}
