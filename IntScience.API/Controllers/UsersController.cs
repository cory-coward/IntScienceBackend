using AutoMapper;
using IntScience.API.Dtos;
using IntScience.Repository.IdentityModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IntScience.API.Controllers;

[Route("api/[controller]")]
//[Authorize(Roles = "Admin")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IMapper _mapper;
    private readonly ILogger<UsersController> _logger;

    public UsersController(UserManager<ApplicationUser> userManager,
                           RoleManager<IdentityRole> roleManager,
                           IMapper mapper,
                           ILogger<UsersController> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<UserProfileResponseDto>))]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userManager.Users.AsNoTracking().ToListAsync();

        var userDtos = _mapper.Map<List<UserProfileResponseDto>>(users);

        return Ok(userDtos);
    }

    [HttpGet("{userId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetUserById(string userId)
    {
        if (string.IsNullOrWhiteSpace(userId)) return BadRequest("The user ID is required.");

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null) return NotFound();

        var userDto = _mapper.Map<UserProfileResponseDto>(user);

        return Ok(userDto);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UserProfileResponseDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateUser([FromBody] UserRegistrationDto userDto)
    {
        if (userDto is null || !ModelState.IsValid) return BadRequest();

        var userToAdd = _mapper.Map<ApplicationUser>(userDto);

        var result = await _userManager.CreateAsync(userToAdd, userDto.Password);

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

        var actionName = nameof(GetUserById);
        var routeParameters = new { userId = userToAdd.Id };
        var createdUser = _mapper.Map<UserProfileResponseDto>(userToAdd);

        return CreatedAtAction(actionName, routeParameters, createdUser);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto userDto)
    {
        if (userDto is null || !ModelState.IsValid) return BadRequest();

        var userToUpdate = await _userManager.FindByIdAsync(userDto.Id);

        if (userToUpdate is null) return NotFound();

        var currentUserRoles = await _userManager.GetRolesAsync(userToUpdate);

        var rolesToAdd = userDto.Roles.Except(currentUserRoles);
        var rolesToRemove = currentUserRoles.Except(userDto.Roles);

        userToUpdate.FirstName = userDto.FirstName;
        userToUpdate.LastName = userDto.LastName;

        await _userManager.UpdateAsync(userToUpdate);
        await _userManager.AddToRolesAsync(userToUpdate, rolesToAdd);
        await _userManager.RemoveFromRolesAsync(userToUpdate, rolesToRemove);

        return NoContent();
    }

    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        if (userId is null) return BadRequest("The user ID is required.");

        var userToDelete = await _userManager.FindByIdAsync(userId);

        if (userToDelete is null) return NotFound();

        await _userManager.DeleteAsync(userToDelete);

        return NoContent();
    }
}
