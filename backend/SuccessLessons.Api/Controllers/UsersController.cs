using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SuccessLessons.Api.DTOs;
using SuccessLessons.Api.Services;

namespace SuccessLessons.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    private string Uid => User.FindFirstValue(ClaimTypes.NameIdentifier)!;
    private string Email => User.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    [HttpGet("me")]
    public async Task<ActionResult<UserProfileDto>> GetMe()
    {
        var user = await _userService.GetOrCreateAsync(Uid, Email, string.Empty);
        return Ok(new UserProfileDto(user.Uid, user.DisplayName, user.Email, user.JoinedAt.ToDateTime(), user.ReadingStreak));
    }

    [HttpPatch("me")]
    public async Task<ActionResult<UserProfileDto>> UpdateMe(UpdateUserProfileRequest request)
    {
        var user = await _userService.UpdateDisplayNameAsync(Uid, request.DisplayName);
        return Ok(new UserProfileDto(user.Uid, user.DisplayName, user.Email, user.JoinedAt.ToDateTime(), user.ReadingStreak));
    }
}
