using AutoMapper;
using IntScience.API.Dtos;
using IntScience.API.Services;
using IntScience.Repository.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly IJwtService _jwtService;

    public AccountController(UserManager<ApplicationUser> userManager,
                             SignInManager<ApplicationUser> signInManager,
                             RoleManager<IdentityRole> roleManager,
                             IMapper mapper,
                             IJwtService jwtService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserLoginResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(UserLoginResponseDto))]
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

    [HttpPost("register")]
    //[Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistrationDto)
    {
        if (userRegistrationDto is null || !ModelState.IsValid) return BadRequest();

        var userToAdd = _mapper.Map<ApplicationUser>(userRegistrationDto);

        var result = await _userManager.CreateAsync(userToAdd, userRegistrationDto.Password);

        if (!result.Succeeded)
        {
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(errors);
        }

        if (await _roleManager.RoleExistsAsync("User") == false)
        {
            var userRole = new IdentityRole { Name = "User", NormalizedName = "USER" };
            await _roleManager.CreateAsync(userRole);
        }

        await _userManager.AddToRoleAsync(userToAdd, "User");

        await _userManager.AddClaimAsync(userToAdd, new Claim(ClaimTypes.NameIdentifier, userToAdd.Id));

        var actionName = nameof(GetUserProfile);
        var routeParameters = new { userId = userToAdd.Id };
        var createdUser = _mapper.Map<UserProfileResponseDto>(userToAdd);

        return CreatedAtAction(actionName, routeParameters, createdUser);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserProfile(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest();

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) return NotFound();

        var profileResponseDto = _mapper.Map<UserProfileResponseDto>(user);

        return Ok(profileResponseDto);
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
