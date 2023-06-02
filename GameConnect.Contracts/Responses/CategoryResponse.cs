using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameConnect.Contracts.Responses;
public class CategoryResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}
