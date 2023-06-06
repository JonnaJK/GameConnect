// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using GameConnect.Domain.Entities;
using GameConnect.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameConnect.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        [BindProperty]
        public IFormFile UploadedImage { get; set; }

        public IndexModel(
            UserManager<User> userManager,
            SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public string UserName { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "About me")]
            public string AboutMe { get; set; }
            public string BackgroundColor { get; set; }
        }

        private async Task LoadAsync(User user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            var resultUser = await _userManager.GetUserAsync(User);
            var aboutMe = resultUser.AboutMe;
            var backgroundColor = resultUser.Background;
            var favoriteGames = resultUser.FavoriteGames;

            UserName = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                AboutMe = aboutMe,
                BackgroundColor = backgroundColor
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var resultUser = await _userManager.GetUserAsync(User);

            resultUser.AboutMe = Input.AboutMe;
            resultUser.PhoneNumber = Input.PhoneNumber;
            resultUser.Background = Input.BackgroundColor;

            if (UploadedImage is not null)
            {
                if (resultUser.ImageUrl != null)
                {
                    FileHelper.RemoveImage($"profilePictures/{resultUser.ImageUrl}");
                }
                var fileName = resultUser.UserName + UploadedImage.FileName;
                await FileHelper.AddImage(UploadedImage, "profilePictures/" + fileName);
                resultUser.ImageUrl = fileName;
            }

            var update = await _userManager.UpdateAsync(resultUser);

            if (!update.Succeeded)
            {
                StatusMessage = "Unexpected error.";
                return RedirectToPage();
            }


            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
