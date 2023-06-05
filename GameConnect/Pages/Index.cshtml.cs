using GameConnect.Domain.Data;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserService _userService;

    public IndexModel(ApplicationDbContext context, RoleManager<IdentityRole> roleManager, UserService userService)
    {
        _context = context;
        _roleManager = roleManager;
        _userService = userService;
    }

    public void OnGet()
    {
        //var jonnaTMUserId = "e2820c68-6a6e-4c35-a195-3acbc69501f3";
        //var friborghUserId = "1b75c8f2-a589-4a5d-87a0-065edf3d0a19";
        //var roleName = "Admin";
        //await AddRoleToDatabase(roleName);
        //await AddRoleToUser(roleName, jonnaTMUserId);
        //await AddRoleToUser(roleName, friborghUserId);
    }

    private async Task AddRoleToDatabase(string roleName)
    {
        var role = new IdentityRole
        {
            Name = "Admin",
        };
        await _roleManager.CreateAsync(role);
        await _context.SaveChangesAsync();
    }

    private async Task AddRoleToUser(string roleName, string userId)
    {
        var user = await _userService.GetUserAsync(userId);
        if (user is null)
            return;

        var roles = await _roleManager.Roles.ToListAsync();
        if (!roles.Any())
            return;

        var role = roles.Where(x => x.Name == roleName).FirstOrDefault();
        if (role is null)
            return;

        await _userService.AddRoleToUserAsync(user, role);
        await _context.SaveChangesAsync();
    }
}