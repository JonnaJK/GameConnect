using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using static System.Net.Mime.MediaTypeNames;

namespace GameConnect.Pages
{
    public class UserHomeModel : PageModel
    {
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly VoteService _voteService;
        private readonly ApplicationDbContext _context;

        public User LoggedInUser { get; set; } = new();
        public bool IsSameUser { get; set; } = true;
        public List<BannedWord> BannedWords { get; set; }

        public UserHomeModel(UserService userService, PostService postService, VoteService voteService, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userService = userService;
            _postService = postService;
            _voteService = voteService;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(User user, int upVotePostId, int downVotePostId, int postId)
        {
            BannedWords = await _context.BannedWord.ToListAsync();
            if (postId != 0)
            {
                var post = await _postService.GetPostAsync(postId);
                if (post != null)
                    return RedirectToPage("PostDetails", new Post { Id = post.Id });
            }

            if (user.UserName == null)
            {
                LoggedInUser = await _userService.GetUserAsync(User);
                if (LoggedInUser != null)
                    LoggedInUser.Posts = await _postService.GetPostsForUserAsync(LoggedInUser);

            }
            else
            {
                LoggedInUser = await _userService.GetUserAsync(User);

                user = await _userService.GetUserByUserNameAsync(user.UserName);
                if (user != null && LoggedInUser != null)
                {
                    if (user.Id != LoggedInUser.Id)
                    {
                        IsSameUser = false;
                        LoggedInUser = user;
                    }
                }
            }

            // Upvote or downvote a post
            if (upVotePostId != 0)
            {
                if (LoggedInUser != null)
                {
                    var isChanged = await _voteService.AddUpVoteOnPostAsync(LoggedInUser.Id, upVotePostId);
                    if (isChanged)
                        await _postService.UpVotePostByIdAsync(upVotePostId);

                    var post = await _postService.GetPostAsync(upVotePostId);
                    user = await _userService.GetUserAsync(post.UserId);
                    if (user != null)
                    {
                        if (user.Id != LoggedInUser.Id)
                        {
                            IsSameUser = false;
                            LoggedInUser = user;
                        }
                    }
                }
            }
            else if (downVotePostId != 0)
            {
                if (LoggedInUser != null)
                {
                    var isChanged = await _voteService.AddDownVoteOnPostAsync(LoggedInUser.Id, downVotePostId);
                    if (isChanged)
                        await _postService.DownVotePostByIdAsync(downVotePostId);

                    var post = await _postService.GetPostAsync(downVotePostId);
                    user = await _userService.GetUserAsync(post.UserId);
                    if (user != null)
                    {
                        if (user.Id != LoggedInUser.Id)
                        {
                            IsSameUser = false;
                            LoggedInUser = user;
                        }
                    }
                }
            }

            return Page();
        }
    }
}
