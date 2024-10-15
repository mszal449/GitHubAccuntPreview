using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace WebsiteApi.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController: ControllerBase
{
    [HttpGet("login")]
    public async void Login(string redirect = "/")
    {
        await HttpContext.ChallengeAsync("github", new AuthenticationProperties
        {
            RedirectUri = redirect
        });
    }
    
    [HttpGet("logout")]
    public async Task<IActionResult> Logout(string redirect = "/")
    {
        await HttpContext.SignOutAsync();
        return Redirect(redirect);
    }

    [HttpGet("status")]
    public async Task<IActionResult> Status()
    {
        if (User.Identity?.IsAuthenticated == false)
        {
            return Ok(new
                {
                    result = "Unauthenticated",
                }
            );
        }
    
        return Ok(new
        {
            result = "Authenticated",
            user = new
            {
                id = User.FindFirstValue("id"),
                username = User.FindFirstValue(ClaimTypes.Name),
                email = User.FindFirstValue(ClaimTypes.Email),
            }
        });
    }
}