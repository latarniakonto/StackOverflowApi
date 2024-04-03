using StackOverflow.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using StackOverflow.Api.Models;
using System.Diagnostics;
using StackOverflow.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;

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
    [Authorize]
    [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
    public async Task<ActionResult> GetTags()
    {
        try
        {
            var tags = await _tagService.GetTagsAsync();
            return View("Index", tags);
        }
        catch (Exception ex)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
        }
    }

    [HttpPost("update")]
    [Authorize]
    [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
    public async Task<ActionResult> UpdateTags()
    {
        try
        {
            var tags = await _tagService.GetOrUpdateAllTagsAsync();
            return View("Index", tags);
        }
        catch (Exception ex)
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = ex.Message });
        }
    }
}
