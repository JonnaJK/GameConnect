using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace GameConnect.Domain.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class Tag
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
