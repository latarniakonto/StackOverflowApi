using StackOverflow.Infrastructure.Clients;
using StackOverflow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace StackOverflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly ITagsClient _tagsClient;

    public TagController(ITagService tagService, ITagsClient tagsClient)
    {
        _tagService = tagService;
        _tagsClient = tagsClient;
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var tag = _tagService.GetTagAsync(id).Result;

        if (tag == null)
        {
            return NotFound();
        }

        return Ok(tag);
    }
}
