using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;
using GameConnect.Domain.Services;
using GameConnect.Helpers;

namespace GameConnect.Pages.Manager.PostManager
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly PostService _postService;

        public User LoggedInUser { get; set; }

        [BindProperty]
        public Post Post { get; set; } = default!;

        [BindProperty]
        public IFormFile? UploadedImage { get; set; }

        public CreateModel(ApplicationDbContext context, User loggedInUser, UserService userService, PostService postService)
        {
            _context = context;
            LoggedInUser = loggedInUser;
            _userService = userService;
            _postService = postService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            LoggedInUser = await _userService.GetUserAsync(User);

            ViewData["Tag"] = new SelectList(_context.Tag, "Id", "Name");
            ViewData["Category"] = new SelectList(_context.Category, "Id", "Name");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (UploadedImage is not null)
            {
                var fileName = $"{Random.Shared.Next(10_001)}_{UploadedImage.FileName}";
                await FileHelper.AddImage(UploadedImage, "postImages/" + fileName);
                Post.ImageUrl = fileName;
            }
            else
            {
                // Fullösning
                ModelState.Remove("UploadedImage");
                Post.ImageUrl = "defaultPost.jpg";
            }

            if (Post.CategoryId == 0)
            {
                Post.CategoryId = null;
            }

            if (Post.TagId == 0)
            {
                Post.TagId = null;
            }

            if (!ModelState.IsValid || _context.Post == null || Post == null)
            {
                var errors = ModelState.Values.SelectMany(x => x.Errors);
                return Page();
            }

            LoggedInUser = await _userService.GetUserAsync(User);

            await _postService.SavePostAsync(Post);

            return RedirectToPage("/UserHome");
        }
    }
}
