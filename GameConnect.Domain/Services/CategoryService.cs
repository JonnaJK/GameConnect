using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Services;

public class CategoryService
{
    private readonly ApplicationDbContext _context;

    public CategoryService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(Category category)
    {
        await _context.Category.AddAsync(category);
        await _context.SaveChangesAsync();
    }

    public async Task<Category> GetByIdAsync(int id)
    {
        return await _context.Category.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<List<Category>> GetAllAsync()
    {
        return await _context.Category.OrderByDescending(x => x.Name).ToListAsync();
    }

    public async Task<bool> UpdateAsync(Category category)
    {
        _context.Category.Update(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await GetByIdAsync(id);
        if (category is null)
        {
            return false;
        }
        _context.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Category?> GetCategoryByName(string name)
    {
        return await _context.Category.FirstOrDefaultAsync(x => x.Name == name);
    }
}
