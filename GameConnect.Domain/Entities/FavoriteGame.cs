using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Entities
{
    public class FavoriteGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
    }
}
