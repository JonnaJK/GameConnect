﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GameConnect.Domain.Services;
using GameConnect.Helpers;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;

namespace GameConnect.Pages.Manager.ReplyManager
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly ReplyService _replyService;

        public User LoggedInUser { get; set; } = new();

        public int PostId { get; set; }
        public int? ReplyId { get; set; }

        [BindProperty]
        public Reply Reply { get; set; } = default!;

        [BindProperty]
        public IFormFile UploadedImage { get; set; }
        public Post ReplyOnPost { get; set; } = new();
        public Reply ReplyOnReply { get; set; } = new();

        public CreateModel(ApplicationDbContext context, UserService userService, PostService postService, ReplyService replyService)
        {
            _context = context;
            _userService = userService;
            _postService = postService;
            _replyService = replyService;
        }

        public async Task<IActionResult> OnGetAsync(Post post, Reply reply)
        {
            LoggedInUser = await _userService.GetUserAsync(User);
            if (reply.PostId != 0)
            {
                ReplyId = reply.Id;
                PostId = reply.PostId;
                ReplyOnReply = await _replyService.GetReplyFromIdAsync(reply.Id);
            }
            else if (reply.PostId == 0)
            {
                PostId = post.Id;
                ReplyId = null;
                ReplyOnPost = await _postService.GetPostAsync(post.Id);
            }
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (UploadedImage is not null)
            {
                var fileName = $"{Random.Shared.Next(10_001)}_{UploadedImage.FileName}";
                await FileHelper.AddImage(UploadedImage, "postImages/" + fileName);
                Reply.ImageUrl = fileName;
            }
            else
            {
                // Fullösning
                ModelState.Remove("UploadedImage");
                Reply.ImageUrl = "defaultPost.jpg";
            }

            if (!ModelState.IsValid || _context.Reply == null || Reply == null)
            {
                //var errors = ModelState.Values.SelectMany(x => x.Errors).ToList();
                return Page();
            }

            LoggedInUser = await _userService.GetUserAsync(User);


            _context.Reply.Add(Reply);
            await _context.SaveChangesAsync();

            var post = await _postService.GetPostAsync(Reply.PostId);

            return RedirectToPage("/PostDetails", post);
            //return Page();
        }
    }
}
