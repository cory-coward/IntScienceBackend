using IntScience.Repository.IdentityModels;

namespace IntScience.API.Services;
public interface IJwtService
{
    Task<string> CreateTokenAsync(ApplicationUser user);
}