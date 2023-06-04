using Azure.Core;
using GameConnect.Api.Mapping;
using GameConnect.Contracts.Requests;
using GameConnect.Domain.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GameConnect.Api.Controllers
{
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly TagService _tagService;

        public TagController(TagService tagService)
        {
            _tagService = tagService;
        }

        [HttpPost(ApiEndpoints.Tag.Create)]
        public async Task<IActionResult> CreateAsync([FromBody] CreateTagRequest request)
        {
            var tag = request.MapToTag();
            await _tagService.CreateAsync(tag);
            return Ok();
        }

        [HttpGet(ApiEndpoints.Tag.GetAll)]
        public async Task<IActionResult> GetAllAsync()
        {
            var tags = await _tagService.GetTagsAsync();
            var tagsResponse = tags.MapToResponse();
            return Ok(tagsResponse);
        }

        [HttpGet(ApiEndpoints.Tag.GetByName)]
        public async Task<IActionResult> GetAsync([FromRoute] string name)
        {
            var tag = await _tagService.GetTagByNameAsync(name);
            if (tag is null)
                return NotFound();

            var response = tag.MapToResponse();
            return Ok(response);
        }

        [HttpPut(ApiEndpoints.Tag.Update)]
        public async Task<IActionResult> UpdateAsync([FromRoute] int id,
            [FromBody] UpdateTagRequest request)
        {
            var tag = request.MapToTag(id);
            var updated = await _tagService.UpdateAsync(tag);
            if (!updated)
            {
                return NotFound();
            }

            var response = tag.MapToResponse();
            return Ok(response);
        }

        [HttpDelete(ApiEndpoints.Tag.Delete)]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var deleted = await _tagService.DeleteAsync(id);
            if (!deleted)
            {
                return NotFound();
            }
            return Ok();
        }
    }
}
