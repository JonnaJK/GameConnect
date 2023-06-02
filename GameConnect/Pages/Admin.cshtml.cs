using GameConnect.Data;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameConnect.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private readonly PostService _postService;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserService _userService;
        private readonly ReplyService _replyService;
        public List<Post> ReportedPosts { get; set; }
        public List<Reply> ReportedReplies { get; set; }

        public AdminModel(ApplicationDbContext context, PostService postService, RoleManager<IdentityRole> roleManager, UserService userService, ReplyService replyService)
        {
            _context = context;
            _postService = postService;
            _roleManager = roleManager;
            _userService = userService;
            _replyService = replyService;
        }

        public async Task<IActionResult> OnGetAsync(int restorePostId, int deletePostId, int restoreReplyId, int deleteReplyId)
        {

            if (restorePostId != 0)
            {
                var post = await _postService.GetPostAsync(restorePostId);
                if (post != null)
                {
                    post.IsReported = false;
                    await _context.SaveChangesAsync();
                }
            }
            if (deletePostId != 0)
            {
                var post = await _postService.GetPostAsync(deletePostId);
                if (post != null)
                {
                    if (post.Replies != null)
                    {
                        foreach (var reply in post.Replies)
                        {
                            _context.Reply.Remove(reply);
                        }
                        _context.Post.Remove(post);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            if (restoreReplyId != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(restoreReplyId);
                if (reply != null)
                {
                    reply.IsReported = false;
                    await _context.SaveChangesAsync();
                }
            }
            if (deleteReplyId != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(deleteReplyId);
                if (reply != null)
                {
                    _context.Reply.Remove(reply);
                    await _context.SaveChangesAsync();
                }
            }

            ReportedPosts = await _postService.GetReportedPostsAsync();
            ReportedReplies = await _replyService.GetReportedRepliesAsync();

            return Page();
        }

    }
}
