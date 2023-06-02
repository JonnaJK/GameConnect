using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameConnect.Domain;
public static class ApplicationServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ApplicationDbContext>();

        // Dependency Injections
        services.AddScoped<CategoryService>();
        services.AddScoped<User>();
        services.AddScoped<UserService>(); // Varför kan man inte göra UserService som en Singleton????
        services.AddScoped<PostService>();
        services.AddScoped<ReplyService>();
        services.AddScoped<FavoriteGameService>();
        services.AddScoped<TagService>();
        services.AddScoped<VoteService>();
        services.AddScoped<SessionService>();
        services.AddScoped<ChatMessageService>();


        return services;
    }
}
