﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.IO;
using GameConnect.Domain.Services;
using GameConnect.Helpers;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;

namespace GameConnect.Pages.Manager.PostManager
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PostService _postService;
        private readonly VoteService _voteService;

        public DeleteModel(ApplicationDbContext context, PostService postService, VoteService voteService)
        {
            _context = context;
            _postService = postService;
            _voteService = voteService;
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }

            var post = await _postService.GetPostAsync(id);

            if (post == null)
            {
                return NotFound();
            }
            else
            {
                Post = post;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Post == null)
            {
                return NotFound();
            }
            var post = await _postService.GetPostAsync(id);

            if (post != null)
            {
                Post = post;
                if (Post.ImageUrl != "/postImages/defaultPost.jpg")
                {
                    FileHelper.RemoveImage(Post.ImageUrl);
                }
                if (Post.Replies != null)
                {
                    foreach (var reply in Post.Replies)
                    {
                        if (reply.Replies != null && reply.Replies.Count > 0)
                        {
                            foreach (var r in reply.Replies)
                            {
                                if (!string.IsNullOrEmpty(r.ImageUrl))
                                {
                                    FileHelper.RemoveImage(r.ImageUrl);
                                }
                                await _voteService.RemoveVotesOnReplyAsync(r);
                                _context.Reply.Remove(r);
                            }
                        }
                        if (!string.IsNullOrEmpty(reply.ImageUrl))
                        {
                            FileHelper.RemoveImage(reply.ImageUrl);
                        }
                        await _voteService.RemoveVotesOnReplyAsync(reply);
                        _context.Reply.Remove(reply);
                    }
                }

                await _voteService.RemoveVotesOnPostAsync(Post);

                _context.Post.Remove(Post);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Forum");
        }
    }
}
