using GameConnect.Contracts.Requests;
using GameConnect.Contracts.Responses;
using GameConnect.Domain.Entities;

namespace GameConnect.Api.Mapping;

public static class ContractMapping
{
    public static Category MapToCategory(this CreateCategoryRequest request)
    {
        return new Category()
        {
            Name = request.Name
        };
    }

    public static CategoryResponse MapToResponse(this Category category)
    {
        return new CategoryResponse()
        {
            Id = category.Id,
            Name = category.Name
        };
    }

    public static CategoriesResponse MapToResponse(this IEnumerable<Category> categories)
    {
        return new CategoriesResponse()
        {
            Items = categories.Select(MapToResponse)
        };
    }

    public static Category MapToCategory(this UpdateCategoryRequest request, int id)
    {
        return new Category()
        {
            Id = id,
            Name = request.Name
        };
    }
}
