using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/bookmarks")]
[Authorize]
public class BookmarksController : ControllerBase
{
    private readonly BookmarkService _bookmarkService;

    public BookmarksController(BookmarkService bookmarkService)
    {
        _bookmarkService = bookmarkService;
    }

    private string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

    [HttpGet]
    public async Task<ActionResult<List<BookmarkDto>>> GetAll()
    {
        var bookmarks = await _bookmarkService.GetForUserAsync(Uid);
        return Ok(bookmarks.Select(b => new BookmarkDto(b.Id, b.ChapterId, b.CreatedAt.ToDateTime())));
    }

    [HttpPost]
    public async Task<ActionResult<BookmarkDto>> Create(CreateBookmarkRequest request)
    {
        var bookmark = await _bookmarkService.AddAsync(Uid, request.ChapterId);
        return Ok(new BookmarkDto(bookmark.Id, bookmark.ChapterId, bookmark.CreatedAt.ToDateTime()));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var deleted = await _bookmarkService.DeleteAsync(Uid, id);
        return deleted ? NoContent() : NotFound();
    }
}
