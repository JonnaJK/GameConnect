using GameConnect.Domain.Services;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace GameConnect.Pages
{
    public class AddFavoriteGamesModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly FavoriteGameService _favoriteGameService;

        [BindProperty]
        public FavoriteGame? FavoriteGame { get; set; }

        public List<FavoriteGame>? FavoriteGames { get; set; } = new List<FavoriteGame>();

        public User? CurrentUser { get; set; }

        public AddFavoriteGamesModel(UserManager<User> userManager, UserService userService, FavoriteGameService favoriteGameService)
        {
            _userManager = userManager;
            _userService = userService;
            _favoriteGameService = favoriteGameService;
        }

        public async Task OnGetAsync(int removeId)
        {
            if (removeId != 0)
            {
                await _favoriteGameService.DeleteFavoriteGameAsync(removeId);
            }

            CurrentUser = await _userService.GetUserAsync(User);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CurrentUser = await _userService.GetUserAsync(User);

            if (CurrentUser != null)
            {
                if (FavoriteGame != null)
                {
                    if (CurrentUser.FavoriteGames == null)
                    {
                        CurrentUser.FavoriteGames = new List<FavoriteGame>();
                    }
                    CurrentUser.FavoriteGames.Add(FavoriteGame);
                    await _userManager.UpdateAsync(CurrentUser);
                }
            }
            return RedirectToPage("/ManageFavoriteGames");
        }
    }
}
