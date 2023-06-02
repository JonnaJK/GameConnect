using GameConnect.Domain.Services;
using GameForum_Inlämningsuppgift.Data;
using GameForum_Inlämningsuppgift.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static System.Net.Mime.MediaTypeNames;

namespace GameConnect.Pages
{
    public class UserHomeModel : PageModel
    {
        private readonly UserManager<User> _userManager;
        private readonly UserService _userService;
        private readonly PostService _postService;
        private readonly VoteService _voteService;
        private readonly ApplicationDbContext _context;

        public User LoggedInUser { get; set; }
        public bool IsSameUser { get; set; } = true;

        public UserHomeModel(UserManager<User> userManager, UserService userService, PostService postService, VoteService voteService, ApplicationDbContext context)
        {
            _userManager = userManager;
            _userService = userService;
            _postService = postService;
            _voteService = voteService;
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(User user, int upVotePostId, int downVotePostId, int postId)
        {
            if (postId != 0)
            {
                var post = await _postService.GetPostAsync(postId);
                return RedirectToPage("PostDetails", post); //new {Post = post, post.Category, post.Tag}
            }

            if (user.UserName == null)
            {
                // Denna if/else del körs när man är inne på någon annans sida och up/down votar deras inlägg. Onödigt då vi ändrar värderna igen i if satserna upVotePostId och downVotePostId
                LoggedInUser = await _userService.GetUserAsync(User);
                LoggedInUser.Posts = await _postService.GetPostsForUserAsync(LoggedInUser);
            }
            else
            {
                LoggedInUser = await _userService.GetUserAsync(User);

                user = await _userService.GetUserAsync(user.Id);
                if (user.Id != LoggedInUser.Id)
                {
                    IsSameUser = false;
                    LoggedInUser = user;
                }
            }

            if (upVotePostId != 0)
            {
                var isChanged = await _voteService.AddUpVoteOnPostAsync(LoggedInUser.Id, upVotePostId);
                if (isChanged)
                    await _postService.UpVotePostByIdAsync(upVotePostId);

                var post = await _postService.GetPostAsync(upVotePostId);
                user = await _userService.GetUserAsync(post.UserId);
                if (user.Id != LoggedInUser.Id)
                {
                    IsSameUser = false;
                    LoggedInUser = user;
                }
            }
            if (downVotePostId != 0)
            {
                var isChanged = await _voteService.AddDownVoteOnPostAsync(LoggedInUser.Id, downVotePostId);
                if (isChanged)
                    await _postService.DownVotePostByIdAsync(downVotePostId);

                var post = await _postService.GetPostAsync(downVotePostId);
                user = await _userService.GetUserAsync(post.UserId);
                if (user.Id != LoggedInUser.Id)
                {
                    IsSameUser = false;
                    LoggedInUser = user;
                }
            }

            return Page();
        }

        public IActionResult ViewPost()
        {
            return RedirectToPage("/PostDetails");
        }
    }
}
