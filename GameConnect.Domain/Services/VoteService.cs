using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services;

public class VoteService
{
    private readonly ApplicationDbContext _context;
    private readonly PostService _postService;
    private readonly ReplyService _replyService;

    public VoteService(ApplicationDbContext context, PostService postService, ReplyService replyService)
    {
        _context = context;
        _postService = postService;
        _replyService = replyService;
    }

    public async Task<bool> AddVoteOnPostAsync(bool isUpvote, string userId, int postId)
    {
        var hasVoted = await CheckIfUserAlreadyHasVotedOnPostAsync(userId, postId);
        if (hasVoted)
        {
            var vote = await GetVoteByUserIdAsync(userId, postId, true);
            if (vote is null)
                return false;
            if (isUpvote)
            {
                if (vote.IsUpvote)
                {
                    return false;
                }
                else
                {
                    vote.IsUpvote = true;
                    vote.IsDownvote = false;
                }
            }
            else
            {
                if (vote.IsDownvote)
                {
                    return false;
                }
                else
                {
                    vote.IsUpvote = false;
                    vote.IsDownvote = true;
                }
            }
        }
        else
        {
            var vote = new Vote
            {
                PostId = postId,
                UserId = userId,
            };
            if (isUpvote)
                vote.IsUpvote = true;
            else
                vote.IsDownvote = true;
            await _context.Vote.AddAsync(vote);
        }
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> AddVoteOnReplyAsync(bool isUpvote, string userId, int replyId)
    {
        var hasVoted = await CheckIfUserAlreadyHasVotedOnReplyAsync(userId, replyId);
        if (hasVoted)
        {
            var vote = await GetVoteByUserIdAsync(userId, replyId, false);
            if (vote is null)
                return false;
            if (isUpvote)
            {
                if (vote.IsUpvote)
                {
                    return false;
                }
                else
                {
                    vote.IsUpvote = true;
                    vote.IsDownvote = false;
                }
            }
            else
            {
                if (vote.IsDownvote)
                {
                    return false;
                }
                else
                {
                    vote.IsUpvote = false;
                    vote.IsDownvote = true;
                }
            }
        }
        else
        {
            var vote = new Vote
            {
                PostId = replyId,
                UserId = userId,
            };
            if (isUpvote)
                vote.IsUpvote = true;
            else
                vote.IsDownvote = true;
            await _context.Vote.AddAsync(vote);
        }
        _context.SaveChanges();
        return true;
    }

    public async Task<bool> CheckIfUserAlreadyHasVotedOnPostAsync(string userId, int postId)
    {
        var vote = await _context.Vote.FirstOrDefaultAsync(x => x.UserId == userId && x.PostId == postId);
        if (vote is null)
            return false;
        return true;
    }

    public async Task<bool> CheckIfUserAlreadyHasVotedOnReplyAsync(string userId, int replyId)
    {
        var vote = await _context.Vote.FirstOrDefaultAsync(x => x.UserId == userId && x.ReplyId == replyId);
        if (vote is null)
            return false;
        return true;
    }

    public async Task<Vote?> GetVoteByUserIdAsync(string userId, int postOrReplyId, bool isPost)
    {
        var votes = await GetUsersVotesByUserIdAsync(userId);
        if (votes == null)
            return null;

        if (isPost)
            return votes.FirstOrDefault(x => x.PostId == postOrReplyId);
        return votes.FirstOrDefault(x => x.ReplyId == postOrReplyId);
    }

    public async Task<List<Vote>?> GetUsersVotesByUserIdAsync(string id)
    {
        return await _context.Vote.Where(x => x.UserId == id).ToListAsync();
    }

    public async Task<List<Vote>?> GetVotesOnPostByPostIdAsync(int id)
    {
        return await _context.Vote.Where(x => x.PostId == id).ToListAsync();
    }

    public async Task<List<Vote>?> GetVotesOnReplyByReplyIdAsync(int id)
    {
        return await _context.Vote.Where(x => x.ReplyId == id).ToListAsync();
    }

    public async Task RemoveVotesOnReplyAsync(Reply reply)
    {
        if (reply.Votes != 0)
        {
            var votes = await GetVotesOnReplyByReplyIdAsync(reply.Id);
            if (votes != null)
            {
                foreach (var vote in votes)
                {
                    _context.Remove(vote);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task RemoveVotesOnPostAsync(Post post)
    {
        if (post.Votes != 0)
        {
            var votes = await GetVotesOnPostByPostIdAsync(post.Id);
            if (votes != null)
            {
                foreach (var vote in votes)
                {
                    _context.Remove(vote);
                }
            }
        }
        await _context.SaveChangesAsync();
    }

    public async Task<Post?> AddVoteOnPost(bool isUpvote, int postId, string userId)
    {
        if (isUpvote)
        {
            var isChanged = await AddVoteOnPostAsync(true, userId, postId);
            if (isChanged)
                await _postService.UpVotePostByIdAsync(postId);
        }
        else
        {
            var isChanged = await AddVoteOnPostAsync(false, userId, postId);
            if (isChanged)
                await _postService.DownVotePostByIdAsync(postId);
        }

        return await _postService.GetPostAsync(postId);
    }

    public async Task<Post?> AddVoteOnReply(bool isUpvote, int replyId, string userId)
    {
        var reply = await _replyService.GetReplyFromIdAsync(replyId);
        if (reply != null)
        {
            if (isUpvote)
            {
                var isChanged = await AddVoteOnReplyAsync(true, userId, replyId);
                if (isChanged)
                    await _replyService.UpVoteReplyAsync(reply);
            }
            else
            {
                var isChanged = await AddVoteOnReplyAsync(false, userId, replyId);
                if (isChanged)
                    await _replyService.DownVoteReplyAsync(reply);
            }
        }
        return await _postService.GetPostAsync(reply.PostId);
    }
}
