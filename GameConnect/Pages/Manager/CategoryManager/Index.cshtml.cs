using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using GameConnect.Contracts.Responses;
using GameConnect.DAL;
using GameConnect.Domain.Data;
using GameConnect.Domain.Entities;
using GameConnect.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace GameConnect.Pages.Manager.CategoryManager;

public class IndexModel : PageModel
{
    private readonly HttpService _httpService;

    public IList<Category> Category { get; set; } = default!;

    public IndexModel(HttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task OnGetAsync()
    {
        var categories = await _httpService.HttpGetRequest<CategoriesResponse>("category");
        Category = categories.MapToCategories();
    }
}