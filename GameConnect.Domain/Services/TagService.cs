using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services;

public class TagService
{
    private readonly ApplicationDbContext _context;

    public TagService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Tag>> GetTagsAsync()
    {
        return await _context.Tag.ToListAsync();
    }

    public async Task<Tag?> GetTagByNameAsync(string name)
    {
        return await _context.Tag.FirstOrDefaultAsync(x => x.Name == name);
    }
}
