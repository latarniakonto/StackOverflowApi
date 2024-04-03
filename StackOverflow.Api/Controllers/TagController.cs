using StackOverflow.Infrastructure.Clients;
using StackOverflow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using StackOverflow.Api.Models;
using System.Diagnostics;

namespace StackOverflow.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TagController : Controller
{
    private readonly ITagService _tagService;

    public TagController(ITagService tagService)
    {
        _tagService = tagService;
    }

    [HttpGet]
    public async Task<ActionResult> GetTags(
        int page = 1,
        int pageSize = 10,
        string nameSortOrder = "asc",
        string weightSortOrder = "asc")
    {
        try
        {
            var tags = await _tagService.GetTagsAsync(page, pageSize, nameSortOrder, weightSortOrder);
            return Ok(tags);
        }
        catch (Exception ex)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
        }
    }

    [HttpPost("update")]
    public async Task<ActionResult> UpdateTags()
    {
        try
        {
            var tags = await _tagService.GetOrUpdateAllTagsAsync();
            return Ok(tags);
        }
        catch (Exception ex)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
        }
    }
}
