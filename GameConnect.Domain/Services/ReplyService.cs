using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services;

public class ReplyService
{
    private readonly ApplicationDbContext _context;

    public ReplyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Reply>> GetReportedRepliesAsync()
    {
        return await _context.Reply.Where(x => x.IsReported).ToListAsync();
    }

    public async Task<List<Reply>> GetRepliesFromPostIdAsync(int id)
    {
        return await _context.Reply
            .Where(x => x.PostId == id)
            .Include(x => x.User)
            .OrderByDescending(x => x.Votes)
            .ToListAsync();
    }

    public async Task<Reply?> GetReplyFromIdAsync(int id)
    {
        return await _context.Reply.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Reply>> GetRepliesFromReplyIdAsync(int id)
    {
        return await _context.Reply
            .Where(x => x.ReplyId == id)
            .Include(x => x.User)
            .ToListAsync();
    }

    public async Task<ICollection<Reply>?> GetRepliesFromReplyList(ICollection<Reply> replies)
    {
        foreach (var reply in replies)
        {
            reply.Replies = await GetRepliesFromReplyIdAsync(reply.Id);
        }
        return replies;
    }

    public async Task UpVoteReplyAsync(Reply reply)
    {
        reply.Votes++;
        await _context.SaveChangesAsync();
    }

    public async Task DownVoteReplyAsync(Reply reply)
    {
        reply.Votes--;
        if (reply.Votes <= -5)
        {
            reply.IsReported = true;
        }
        await _context.SaveChangesAsync();
    }
}
