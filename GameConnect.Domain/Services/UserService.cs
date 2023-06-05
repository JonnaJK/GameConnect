using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameConnect.Domain.Services
{
    public class UserService
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly FavoriteGameService _favoriteGameService;
        private readonly PostService _postService;

        public UserService(UserManager<User> userManager, ApplicationDbContext context, FavoriteGameService favoriteGameService, PostService postService)
        {
            _userManager = userManager;
            _context = context;
            _favoriteGameService = favoriteGameService;
            _postService = postService;
        }

        public async Task<User?> GetUserAsync(ClaimsPrincipal identityUser)
        {
            var user = await _userManager.GetUserAsync(identityUser);
            if (user == null)
                return null;
            user.FavoriteGames = await _favoriteGameService.GetUsersFavoriteGamesAsync(user);
            return user;
        }

        public async Task<User?> GetUserAsync(string id)
        {
            var user = await _context.User
                .Include(x => x.Posts)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return null;
            if (user.Posts != null)
            {
                user.Posts = await _postService.SetPostsAsync(user.Posts);
            }
            user.FavoriteGames = await _favoriteGameService.GetUsersFavoriteGamesAsync(user);
            return user;
        }

        public async Task AddRoleToUserAsync(User? user, IdentityRole identityRole)
        {
            (await _userManager.Users.FirstOrDefaultAsync(x => x.Id == user.Id)).IsAdmin = true;
            await _userManager.AddToRoleAsync(user, identityRole.Name);
            await _context.SaveChangesAsync();
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.User.ToListAsync();
        }

        public async Task<User?> GetUserByUserNameAsync(string userName)
        {
            var user = await _context.User
                .Include(x => x.Posts)
                .FirstOrDefaultAsync(x => x.UserName == userName);
            if (user == null)
                return null;
            if (user.Posts != null)
            {
                user.Posts = await _postService.SetPostsAsync(user.Posts);
            }
            user.FavoriteGames = await _favoriteGameService.GetUsersFavoriteGamesAsync(user);
            return user;
        }
    }
}
