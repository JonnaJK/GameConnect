using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services
{
    public class PostService
    {
        private readonly ApplicationDbContext _context;
        private readonly ReplyService _replyService;

        public PostService(ApplicationDbContext context, ReplyService replyService)
        {
            _context = context;
            _replyService = replyService;
        }
        public async Task<List<Post>> GetPostsAsync()
        {
            return await _context.Post
                .OrderByDescending(x => x.Date)
                .Include(x => x.Replies)
                .Include(x => x.Tag)
                .Include(x => x.Category)
                .Include(x => x.User)
                .ToListAsync();
        }

        public async Task<List<Post>> GetPostsForUserAsync(User user)
        {
            return (await GetPostsAsync()).Where(x => x.UserId == user.Id).ToList();
        }


        public async Task<List<Post>?> GetPostsByCategoryAsync(int categoryId)
        {
            return (await GetPostsAsync()).Where(x => x.CategoryId == categoryId).ToList();
        }

        public async Task<List<Post>?> GetPostsByTagAsync(int tagId)
        {
            return (await GetPostsAsync()).Where(x => x.TagId == tagId).ToList();
        }

        public async Task<Post?> GetPostAsync(int? id)
        {
            // Det utkommenterade gjorde en varning, ser ingen skillnad när jag tog bort den, vi sätter en user i replyservice. Har kvar tillsvidare
            var post = await _context.Post
                .Where(x => x.Id == id)
                .Include(x => x.Tag)
                .Include(x => x.Category)
                .Include(x => x.User)
                .Include(x => x.Replies)
                //.ThenInclude(x => x.User)
                .FirstOrDefaultAsync();

            if (post != null && post.Replies != null)
                post.Replies = await _replyService.GetRepliesFromReplyList(post.Replies);

            return post;
        }

        public async Task<List<Post>> GetReportedPostsAsync()
        {
            return await _context.Post.Where(x => x.IsReported).ToListAsync();
        }

        public async Task<Post> SetPostAsync(Post post)
        {
            post.Category = await _context.Category.FirstOrDefaultAsync(x => x.Id == post.CategoryId);
            post.Tag = await _context.Tag.FirstOrDefaultAsync(x => x.Id == post.TagId);
            post.User = await _context.User.FirstOrDefaultAsync(x => x.Id == post.UserId);
            return post;
        }

        public async Task<ICollection<Post>> SetPostsAsync(ICollection<Post> posts)
        {
            foreach (var post in posts)
            {
                post.Category = await _context.Category.FirstOrDefaultAsync(x => x.Id == post.CategoryId);
                post.Tag = await _context.Tag.FirstOrDefaultAsync(x => x.Id == post.TagId);
                post.User = await _context.User.FirstOrDefaultAsync(x => x.Id == post.UserId);
                post.Replies = await _replyService.GetRepliesFromPostIdAsync(post.Id);
            }
            return posts.OrderByDescending(x => x.Date).ToList();
        }

        public async Task SavePostAsync(Post post)
        {
            _context.Post.Add(post);
            await _context.SaveChangesAsync();
        }

        public async Task UpVotePostByIdAsync(int id)
        {
            var post = _context.Post.FirstOrDefault(x => x.Id == id);
            if (post != null)
            {
                post.Votes++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DownVotePostByIdAsync(int id)
        {
            var post = _context.Post.FirstOrDefault(x => x.Id == id);
            if (post != null)
            {
                post.Votes--;
                if (post.Votes <= -5)
                {
                    post.IsReported = true;
                }
                await _context.SaveChangesAsync();
            }
        }
    }
}
