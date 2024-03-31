using StackOverflow.Infrastructure.Clients;
using StackOverflow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace StackOverflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : ControllerBase
{
    private readonly ITagService _tagService;
    private readonly IClientFactory _clientFactory;

    public TagController(ITagService tagService, IClientFactory clientFactory)
    {
        _tagService = tagService;
        _clientFactory = clientFactory;
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
