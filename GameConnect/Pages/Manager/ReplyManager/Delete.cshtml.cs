using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;
using GameConnect.Domain.Services;
using GameConnect.Helpers;

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

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Reply == null)
            {
                return NotFound();
            }

            var reply = await _context.Reply.FirstOrDefaultAsync(m => m.Id == id);

            if (reply == null)
            {
                return NotFound();
            }
            else
            {
                Reply = reply;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Reply == null)
            {
                return NotFound();
            }
            var reply = await _context.Reply.FindAsync(id);
            reply.Replies = await _replyService.GetRepliesFromReplyIdAsync(reply.Id);

            if (reply != null)
            {
                Reply = reply;
                foreach (var r in Reply.Replies)
                {
                    if (!string.IsNullOrEmpty(r.ImageUrl))
                    {
                        FileHelper.RemoveImage(r.ImageUrl);
                    }
                    await _voteService.RemoveVotesOnReplyAsync(r);

                    _context.Reply.Remove(r);
                }
                if (!string.IsNullOrEmpty(Reply.ImageUrl))
                {
                    FileHelper.RemoveImage(Reply.ImageUrl);
                }
                await _voteService.RemoveVotesOnReplyAsync(Reply);

                _context.Reply.Remove(Reply);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Forum");
        }
    }
}
