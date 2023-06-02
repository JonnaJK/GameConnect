using GameConnect.DAL;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GameConnect.Pages
{
    public class ForumModel : PageModel
    {
        private readonly HttpService _httpService;

        private readonly PostService _postService;
        private readonly TagService _tagService;
        private readonly VoteService _voteService;
        private readonly UserService _userService;
        public List<Post> AllPosts { get; set; }

        public List<Tag> AllTags { get; set; }
        [BindProperty]
        public Category Category { get; set; }

        [BindProperty]
        public string TagName { get; set; }

        public ForumModel(PostService postService, TagService tagService, VoteService voteService, UserService userService, HttpService httpService)
        {
            _postService = postService;
            _tagService = tagService;
            _voteService = voteService;
            _userService = userService;
            _httpService = httpService;
        }

        public async Task<IActionResult> OnGetAsync(int upVotePostId, int downVotePostId, int postId, string categoryName, string tagName, string creatorUser)
        {
            if (!string.IsNullOrEmpty(creatorUser))
            {
                var userToHome = await _userService.GetUserAsync(creatorUser);
                return RedirectToPage("/UserHome", userToHome);
            }

            //AllTags = await _httpService.HttpGetRequest<List<Tag>>("tag");
            AllTags = await _tagService.GetTagsAsync();
            //Category.Name = string.Empty;

            if (postId != 0)
            {
                var post = await _postService.GetPostAsync(postId);
                return RedirectToPage("PostDetails", post); //new {Post = post, post.Category, post.Tag} - Om man lyckas på detta sätt hoppas jag att url:en inte blir fucked
            }

            if (!string.IsNullOrEmpty(categoryName))
            {
                //var category = await _categoryService.GetCategoryByName(categoryName);
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
            var tagCategory = TagName.Split('-');
            var tag = await _tagService.GetTagByNameAsync(tagCategory[0]);
            if (tagCategory[1] != string.Empty)
            {
                //var category = await _categoryService.GetCategoryByName(tagCategory[1]);
                var category = await _httpService.HttpGetRequest<Category>("category/" + tagCategory[1]);
                if (category != null)
                {
                    Category = category;
                    AllPosts = (await _postService.GetPostsByCategoryAsync(category.Id)).Where(x => x.TagId == tag.Id).ToList();
                }
            }
            else
            {
                AllPosts = (await _postService.GetPostsAsync()).Where(x => x.TagId == tag.Id).ToList();
            }

            AllTags = await _tagService.GetTagsAsync();
        }
    }
}
