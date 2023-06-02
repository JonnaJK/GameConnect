using GameConnect.Api.Mapping;
using GameConnect.Contracts.Requests;
using GameConnect.Domain.Entities;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameConnect.Api.Controllers;

[ApiController]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(CategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpPost(ApiEndpoints.Category.Create)]
    public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryRequest request)
    {
        var category = request.MapToCategory();
        await _categoryService.CreateAsync(category);
        return Ok();
    }

    [HttpGet(ApiEndpoints.Category.GetById)]
    public async Task<IActionResult> GetAsync([FromRoute] int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        if (category is null)
        {
            return NotFound();
        }
        var response = category.MapToResponse();
        return Ok(response);
    }

    //[HttpGet(ApiEndpoints.Category.GetByName)]
    //public async Task<IActionResult> GetAsync([FromRoute] string name)
    //{
    //    var category = await _categoryService.GetCategoryByName(name);
    //    if (category is null)
    //    {
    //        return NotFound();
    //    }
    //    var response = category.MapToResponse();
    //    return Ok(response);
    //}

    [HttpGet(ApiEndpoints.Category.GetAll)]
    public async Task<IActionResult> GetAllAsync()
    {
        var categories = await _categoryService.GetAllAsync();

        var categoryResponse = categories.MapToResponse();
        return Ok(categoryResponse);
    }

    [HttpPut(ApiEndpoints.Category.Update)]
    public async Task<IActionResult> UpdateAsync([FromRoute] int id,
        [FromBody] UpdateCategoryRequest request)
    {
        var category = request.MapToCategory(id);
        var updated = await _categoryService.UpdateAsync(category);
        if (!updated)
        {
            return NotFound();
        }

        var response = category.MapToResponse();
        return Ok(response);
    }

    [HttpDelete(ApiEndpoints.Category.Delete)]
    public async Task<IActionResult> DeleteAsync([FromRoute] int id)
    {
        var deleted = await _categoryService.DeleteAsync(id);
        if (!deleted)
        {
            return NotFound();
        }
        return Ok();
    }
}
