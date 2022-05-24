namespace IntScience.API.Services;

public interface IUserService
{
    Task CreateDefaultRolesAsync();
    Task CreateDefaultUserAsync();
}