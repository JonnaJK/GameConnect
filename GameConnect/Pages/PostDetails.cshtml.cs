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

        public Post? Post { get; set; } = new();
        public User LoggedInUser { get; set; } = new();
        public bool IsSameUser { get; set; }
        public List<BannedWord> BannedWords { get; set; } = new();

        public PostDetailsModel(PostService postService, UserService userService, ApplicationDbContext context, ReplyService replyService, VoteService voteService, SignInManager<User> signInManager)
        {
            _postService = postService;
            _userService = userService;
            _context = context;
            _replyService = replyService;
            _voteService = voteService;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> OnGetAsync(Post? post, int upVotePostId, int downVotePostId, int upVoteReplyId, int downVoteReplyId, int postId, int replyId, string creatorUser, int reportedPostId, int reportedReplyId)
        {
            BannedWords = await _context.BannedWord.ToListAsync();
            if (!_signInManager.IsSignedIn(User))
                return Page();

            // Reply on post or reply on reply
            if (postId != 0)
            {
                post = await _postService.GetPostAsync(postId);
                if (post != null)
                    return RedirectToPage("/Manager/ReplyManager/Create", new Post { Id = post.Id });
            }
            else if (replyId != 0)
            {
                var reply = await _replyService.GetReplyFromIdAsync(replyId);
                if (reply != null)
                    return RedirectToPage("/Manager/ReplyManager/Create", new Reply { Id = reply.Id, PostId = reply.PostId });
            }

            // Go to a users page
            if (!string.IsNullOrEmpty(creatorUser))
            {
                var user = await _userService.GetUserAsync(creatorUser);
                if (user != null)
                    return RedirectToPage("/UserHome", new User { UserName = user.UserName });
            }

            // Upvote or downvote a post
            if (upVotePostId != 0)
            {
                var isChanged = await _voteService.AddUpVoteOnPostAsync(LoggedInUser.Id, upVotePostId);
                if (isChanged)
                    await _postService.UpVotePostByIdAsync(upVotePostId);

                post = await _postService.GetPostAsync(upVotePostId);
            }
            else if (downVotePostId != 0)
            {
                var isChanged = await _voteService.AddDownVoteOnPostAsync(LoggedInUser.Id, downVotePostId);
                if (isChanged)
                    await _postService.DownVotePostByIdAsync(downVotePostId);

                post = await _postService.GetPostAsync(downVotePostId);
            }

            // Upvote och downvote a reply
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
            else if (downVoteReplyId != 0)
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

            // Repost a post or a reply
            if (reportedPostId != 0)
            {
                post = await _postService.GetPostAsync(reportedPostId);
                if (post != null)
                {
                    post.IsReported = true;
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage("/Forum");
            }
            else if (reportedReplyId != 0)
            {
                var reportedReply = await _replyService.GetReplyFromIdAsync(reportedReplyId);
                if (reportedReply != null)
                {
                    post = await _postService.GetPostAsync(reportedReply.PostId);
                    reportedReply.IsReported = true;
                    await _context.SaveChangesAsync();
                }
                return RedirectToPage("/Forum");
            }

            post = await _postService.GetPostAsync((post != null ? post.Id : 0));
            if (post != null)
            {

                var postUser = await _userService.GetUserAsync(post.UserId);
                if (postUser != null && postUser.Id == LoggedInUser.Id)
                {
                    IsSameUser = true;
                }

                Post = await _postService.GetPostAsync(post.Id);
            }
            return Page();
        }
    }
}
