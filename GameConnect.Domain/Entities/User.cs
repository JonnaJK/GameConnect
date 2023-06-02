using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace GameConnect.Domain.Entities
{
    public class User : IdentityUser
    {
        public string? ImageUrl { get; set; }

        public string? AboutMe { get; set; }
        public bool IsAdmin { get; set; }

        public virtual ICollection<FavoriteGame>? FavoriteGames { get; set; }
        public virtual ICollection<Reply>? Replies { get; set; }
        public virtual ICollection<Post>? Posts { get; set; }
        public virtual ICollection<Session>? Sessions { get; set; }

        public string? Background { get; set; }

    }
}
