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
}
