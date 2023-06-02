using Microsoft.EntityFrameworkCore;

namespace GameConnect.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
