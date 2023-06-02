using GameConnect.Api.Mapping;
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
    }
}
