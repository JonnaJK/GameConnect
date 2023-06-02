using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameConnect.Contracts.Responses
{
    public class TagsResponse
    {
        public required IEnumerable<TagResponse> Tags { get; init; } = Enumerable.Empty<TagResponse>();

    }
}
