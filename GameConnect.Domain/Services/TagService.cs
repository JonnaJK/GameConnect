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

    public async Task<Tag?> GetByIdAsync(int id)
    {
        return await _context.Tag.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task CreateAsync(Tag tag)
    {
        await _context.Tag.AddAsync(tag);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateAsync(Tag tag)
    {
        _context.Tag.Update(tag);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tag = await GetByIdAsync(id);
        if (tag is null)
        {
            return false;
        }
        _context.Remove(tag);
        await _context.SaveChangesAsync();
        return true;
    }
}
