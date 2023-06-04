using GameConnect.DAL;
using GameConnect.Data;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace GameConnect;
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));
        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        // Prevents not admin users to access everything in Manager folder
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin",
                policy => policy.RequireRole("Admin"));
        });
        builder.Services.AddRazorPages(options =>
            options.Conventions.AuthorizeFolder("/Manager", "Admin"));

        // Dependency Injections
        builder.Services.AddScoped<User>();

        builder.Services.AddScoped<UserService>(); // Varför kan man inte göra UserService som en Singleton????
        builder.Services.AddScoped<PostService>();
        builder.Services.AddScoped<ReplyService>();
        builder.Services.AddScoped<FavoriteGameService>();
        builder.Services.AddScoped<VoteService>();
        builder.Services.AddScoped<SessionService>();
        builder.Services.AddScoped<ChatMessageService>();
        builder.Services.AddScoped<HttpService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapRazorPages();

        app.Run();
    }
}
