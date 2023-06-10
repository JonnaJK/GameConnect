using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameConnect.Domain.Services;
using GameConnect.Helpers;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;

namespace GameConnect.Pages.Manager.ReplyManager
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PostService _postService;
        private readonly ReplyService _replyService;
        private readonly VoteService _voteService;

        public DeleteModel(ApplicationDbContext context, PostService postService, ReplyService replyService, VoteService voteService)
        {
            _context = context;
            _postService = postService;
            _replyService = replyService;
            _voteService = voteService;
        }

        [BindProperty]
        public Reply Reply { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int id, Reply replyToDelete)
        {
            if (id != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(id);
                if (reply != null)
                {
                    Reply = reply;
                }
            }
            else if (replyToDelete != null && replyToDelete.Id != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(replyToDelete.Id);
                if (reply != null)
                {
                    Reply = reply;
                }
            }
            else
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var reply = await _context.Reply.FindAsync(id);
            if (reply != null)
            {
                reply.Replies = await _replyService.GetRepliesFromReplyIdAsync(reply.Id);

                Reply = reply;
                foreach (var r in Reply.Replies)
                {
                    if (r.ImageUrl != "defaultPost.jpg")
                    {
                        FileHelper.RemoveImage($"postImages/{r.ImageUrl}");
                    }
                    await _voteService.RemoveVotesOnReplyAsync(r);

                    _context.Reply.Remove(r);
                }
                if (Reply.ImageUrl != "defaultPost.jpg")
                {
                    FileHelper.RemoveImage($"postImages/{Reply.ImageUrl}");
                }
                await _voteService.RemoveVotesOnReplyAsync(Reply);

                _context.Reply.Remove(Reply);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Forum");
        }
    }
}
