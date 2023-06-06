using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GameConnect.Domain.Entities;

namespace GameConnect.Domain.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; } = default!;
        public DbSet<Post> Post { get; set; } = default!;
        public DbSet<Reply> Reply { get; set; } = default!;
        public DbSet<Category> Category { get; set; } = default!;
        public DbSet<Tag> Tag { get; set; } = default!;
        public DbSet<FavoriteGame> FavoriteGame { get; set; } = default!;
        public DbSet<ChatMessage> ChatMessage { get; set; } = default!;
        public DbSet<ChatMessageStatus> ChatMessageStatus { get; set; } = default!;
        public DbSet<Session> Session { get; set; } = default!;
        public DbSet<Vote> Vote { get; set; } = default!;
        public DbSet<BannedWord> BannedWord { get; set; } = default!;
    }
}