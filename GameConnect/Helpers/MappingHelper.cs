using GameConnect.Contracts.Responses;
using GameConnect.Domain.Entities;

namespace GameConnect.Helpers;

public static class MappingHelper
{
    public static List<Category> MapToCategories(this CategoriesResponse categoriesResponse)
    {
        return categoriesResponse.Items.Select(x => new Category { Id = x.Id, Name = x.Name }).ToList();
    }
    public static List<Tag> MapToTags(this TagsResponse tagsResponse)
    {
        return tagsResponse.Items.Select(x => new Tag { Id = x.Id, Name = x.Name }).ToList();
    }
}
