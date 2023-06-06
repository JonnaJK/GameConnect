using GameConnect.Contracts.Responses;
using GameConnect.DAL;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using GameConnect.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages;

public class ForumModel : PageModel
{
    private readonly HttpService _httpService;
    private readonly PostService _postService;
    private readonly VoteService _voteService;
    private readonly UserService _userService;
    private readonly ApplicationDbContext _context;

    [BindProperty]
    public Category? Category { get; set; }
    [BindProperty]
    public string? TagAndCategoryNames { get; set; }
    public List<Post>? AllPosts { get; set; }
    public List<Tag> AllTags { get; set; } = new List<Tag>();
    public List<BannedWord> BannedWords { get; set; }

    public ForumModel(PostService postService, VoteService voteService, UserService userService, HttpService httpService, ApplicationDbContext context)
    {
        _postService = postService;
        _voteService = voteService;
        _userService = userService;
        _httpService = httpService;
        _context = context;
    }

    public async Task<IActionResult> OnGetAsync(int upVotePostId, int downVotePostId, int postId, string categoryName, string tagName, string creatorUser)
    {       
        BannedWords = await _context.BannedWord.ToListAsync();

        if (!string.IsNullOrEmpty(creatorUser))
        {
            var userToVisit = await _userService.GetUserAsync(creatorUser);
            if (userToVisit != null)
                return RedirectToPage("/UserHome", new User { UserName = userToVisit.UserName });
        }

        var tags = await _httpService.HttpGetRequest<TagsResponse>("tag");
        if (tags != null)
        {
            AllTags = tags.MapToTags();
        }

        if (postId != 0)
        {
            var post = await _postService.GetPostAsync(postId);
            if (post != null)
                return RedirectToPage("PostDetails", new Post { Id = post.Id });
        }

        if (!string.IsNullOrEmpty(categoryName))
        {
            var category = await _httpService.HttpGetRequest<Category>("category/" + categoryName);
            if (category != null)
            {
                Category = category;
                AllPosts = await _postService.GetPostsByCategoryAsync(category.Id);
            }
        }
        else
        {
            AllPosts = await _postService.GetPostsAsync();
        }

        if (!string.IsNullOrEmpty(tagName))
        {
            var tag = await _httpService.HttpGetRequest<Tag>("tag/" + tagName);
            if (tag != null)
            {
                if (AllPosts != null)
                    AllPosts = AllPosts.Where(x => x.TagId == tag.Id).ToList();
            }
        }

        var user = await _userService.GetUserAsync(User);
        if (user != null)
        {
            if (upVotePostId != 0)
            {
                var isChanged = await _voteService.AddUpVoteOnPostAsync(user.Id, upVotePostId);
                if (isChanged)
                    await _postService.UpVotePostByIdAsync(upVotePostId);
            }
            if (downVotePostId != 0)
            {
                var isChanged = await _voteService.AddDownVoteOnPostAsync(user.Id, downVotePostId);
                if (isChanged)
                    await _postService.DownVotePostByIdAsync(downVotePostId);
            }
        }

        return Page();
    }

    public async Task OnPostAsync()
    {
        if (string.IsNullOrEmpty(TagAndCategoryNames))
            return;
        var tagAndCategoryNames = TagAndCategoryNames.Split('-');
        var tag = await _httpService.HttpGetRequest<Tag>($"tag/{tagAndCategoryNames[0]}");
        if (tagAndCategoryNames[1] != string.Empty)
        {
            var category = await _httpService.HttpGetRequest<Category>("category/" + tagAndCategoryNames[1]);
            if (category != null)
            {
                Category = category;
                AllPosts = (await _postService.GetPostsByCategoryAsync(category.Id))?.Where(x => x.TagId == tag?.Id).ToList();
            }
        }
        else
        {
            AllPosts = (await _postService.GetPostsAsync()).Where(x => x.TagId == tag?.Id).ToList();
        }
        var tags = await _httpService.HttpGetRequest<TagsResponse>("tag");
        if (tags == null)
            return;
        AllTags = tags.MapToTags();
    }
}
