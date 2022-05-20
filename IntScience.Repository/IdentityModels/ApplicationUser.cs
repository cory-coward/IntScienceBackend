using Microsoft.AspNetCore.Identity;

namespace IntScience.Repository.IdentityModels;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
}
