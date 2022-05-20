using IntScience.Repository.IdentityModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace IntScience.API.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserService> _logger;

    public UserService(UserManager<ApplicationUser> userManager,
                       RoleManager<IdentityRole> roleManager,
                       ILogger<UserService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task CreateDefaultUserAsync()
    {
        var userCount = _userManager.Users.Count();

        if (userCount == 0)
        {
            var defaultUser = new ApplicationUser
            {
                FirstName = "DefaultAppUser",
                LastName = "DefaultAppUser",
                UserName = "defaultappuser",
                Email = "default@default.com"
            };

            var result = await _userManager.CreateAsync(defaultUser, "Abcd!234");

            if (!result.Succeeded)
            {
                _logger.LogError("Could not create default user");
                return;
            }

            if (await _roleManager.RoleExistsAsync("User") == false)
            {
                var userRole = new IdentityRole { Name = "User", NormalizedName = "USER" };
                await _roleManager.CreateAsync(userRole);
            }

            await _userManager.AddToRoleAsync(defaultUser, "User");
            await _userManager.AddClaimAsync(defaultUser, new Claim(ClaimTypes.NameIdentifier, defaultUser.Id));
        }
    }
}
