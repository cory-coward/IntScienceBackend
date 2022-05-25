using IntScience.API.Dtos;
using IntScience.API.Services;
using IntScience.Repository.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace IntScience.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtService _jwtService;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserLoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserLoginDto userLoginDto)
    {
        if (userLoginDto is null || !ModelState.IsValid) return BadRequest();

        var user = await _userManager.FindByEmailAsync(userLoginDto.Email);

        if (user is null)
        {
            return Unauthorized(new UserLoginResponseDto
            {
                IsAuthenticationSuccessful = false,
                ErrorMessage = "User account not found"
            });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, userLoginDto.Password, false);

        if (!result.Succeeded)
        {
            return Unauthorized(new UserLoginResponseDto
            {
                IsAuthenticationSuccessful = false,
                ErrorMessage = "Invalid username or password"
            });
        }

        var loginResponseDto = await CreateUserLoginResponseDtoAsync(user);

        return Ok(loginResponseDto);
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserLoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.FindByEmailAsync(User.FindFirstValue(ClaimTypes.Email));

        if (user is null) return NotFound();

        var userDto = await CreateUserLoginResponseDtoAsync(user);
        return Ok(userDto);
    }

    private async Task<UserLoginResponseDto> CreateUserLoginResponseDtoAsync(ApplicationUser user)
    {
        var token = await _jwtService.CreateTokenAsync(user);

        var roles = await _userManager.GetRolesAsync(user);

        return new UserLoginResponseDto
        {
            IsAuthenticationSuccessful = true,
            ErrorMessage = String.Empty,
            UserId = user.Id,
            Email = user.Email,
            Roles = roles,
            Token = token
        };
    }
}
