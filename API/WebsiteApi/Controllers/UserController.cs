using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebsiteApi.Context;
using WebsiteApi.Models;
using WebsiteApi.Services;

namespace WebsiteApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController(UserService service) : Controller
{
    private readonly UserService _userService = service; 
    
    [HttpGet("{userId:int}")]
    public async Task<IActionResult> GetUser(int userId)
    {
        var user = await _userService.GetUser(userId);
        return user is null
            ? Problem(statusCode: StatusCodes.Status404NotFound, title: $"User not found (product id: {userId})")
            : Ok(UserResponse.FromModel(user));
    }


    public record UserResponse(
        int Id,
        string UserName,
        string Email,
        decimal CurrentBalance
    )
    {
        public static UserResponse FromModel(User user)
        {
            return new UserResponse(
                user.UserId,
                user.DisplayName,
                user.Email,
                user.CurrencyBalance
            );
        }
    }
}