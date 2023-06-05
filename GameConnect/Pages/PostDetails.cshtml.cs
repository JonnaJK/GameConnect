using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages
{
    public class PostDetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly PostService _postService;
        private readonly UserService _userService;
        private readonly ReplyService _replyService;
        private readonly VoteService _voteService;
        private readonly SignInManager<User> _signInManager;

        public Post Post { get; set; }
        public User LoggedInUser { get; set; }
        public bool IsSameUser { get; set; }

        public PostDetailsModel(PostService postService, UserService userService, ApplicationDbContext context, ReplyService replyService, VoteService voteService, SignInManager<User> signInManager)
        {
            _postService = postService;
            _userService = userService;
            _context = context;
            _replyService = replyService;
            _voteService = voteService;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync(Post post, int upVotePostId, int downVotePostId, int upVoteReplyId, int downVoteReplyId, int postId, int replyId, string creatorUser, int reportedPostId, int reportedReplyId)
        {
            if (!_signInManager.IsSignedIn(User))
                return Page();
            LoggedInUser = await _userService.GetUserAsync(User);
            if (postId != 0)
            {
                post = await _postService.GetPostAsync(postId);
                return RedirectToPage("/Manager/ReplyManager/Create", post);
            }
            if (replyId != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(replyId);
                return RedirectToPage("/Manager/ReplyManager/Create", reply);
            }
            if (!string.IsNullOrEmpty(creatorUser))
            {
                var user = await _userService.GetUserAsync(creatorUser);
                return RedirectToPage("/UserHome", user);
            }

            if (upVotePostId != 0)
            {
                var isChanged = await _voteService.AddUpVoteOnPostAsync(LoggedInUser.Id, upVotePostId);
                if (isChanged)
                    await _postService.UpVotePostByIdAsync(upVotePostId);

                post = await _postService.GetPostAsync(upVotePostId);
            }
            if (downVotePostId != 0)
            {
                var isChanged = await _voteService.AddDownVoteOnPostAsync(LoggedInUser.Id, downVotePostId);
                if (isChanged)
                    await _postService.DownVotePostByIdAsync(downVotePostId);

                post = await _postService.GetPostAsync(downVotePostId);
            }

            if (upVoteReplyId != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(upVoteReplyId);
                if (reply != null)
                {
                    var isChanged = await _voteService.AddUpVoteOnReplyAsync(LoggedInUser.Id, upVoteReplyId);
                    if (isChanged)
                        await _replyService.UpVoteReplyAsync(reply);

                    post = await _postService.GetPostAsync(reply.PostId);
                }
            }
            if (downVoteReplyId != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(downVoteReplyId);
                if (reply != null)
                {
                    var isChanged = await _voteService.AddDownVoteOnReplyAsync(LoggedInUser.Id, downVoteReplyId);
                    if (isChanged)
                        await _replyService.DownVoteReplyAsync(reply);

                    post = await _postService.GetPostAsync(reply.PostId);
                }
            }

            if (reportedPostId != 0)
            {
                post = await _postService.GetPostAsync(reportedPostId);
                post.IsReported = true;
                await _context.SaveChangesAsync();
                return RedirectToPage("/Forum");
            }
            if (reportedReplyId != 0)
            {
                var reportedReply = await _replyService.GetReplyFromIdAsync(reportedReplyId);
                post = await _postService.GetPostAsync(reportedReply.PostId);
                reportedReply.IsReported = true;
                await _context.SaveChangesAsync();
                return RedirectToPage("/Forum");
            }
            post = await _postService.GetPostAsync(post.Id);
            var postUser = await _userService.GetUserAsync(post.UserId);
            if (postUser.Id == LoggedInUser.Id)
            {
                IsSameUser = true;
            }

            Post = await _postService.GetPostAsync(post.Id);
            return Page();
        }
    }
}
