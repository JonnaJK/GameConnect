using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GameConnect.Contracts.Responses;
public class CategoriesResponse
{
    public required IEnumerable<CategoryResponse> Items { get; init; } = Enumerable.Empty<CategoryResponse>();
}
