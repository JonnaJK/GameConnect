using System.Text.Json.Serialization;

namespace GameConnect.Domain.Entities
{
    public class Category
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }
    }
}
